using AJudge.Application.DtO.BlogDTO;
using AJudge.Application.DtO.CommentDTO;
using AJudge.Application.services;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfwork;
        private readonly ICommentService _commentService;
        public CommentController(IUnitOfWork unitOfwork, ICommentService commentService)
        {
            _unitOfwork = unitOfwork;
            _commentService= commentService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            Comment? comment = await _unitOfwork.Comment.GetById(x => x.Id==id, new[] {nameof(Comment.Blog),nameof(Comment.User),nameof(Comment.Votes)});
            if(comment == null)
                return NotFound("No such comment");

            var commentResponse=CommentResponseDTO.ConvertToCommentResponseDTO(comment);    

            return Ok(commentResponse);

        }

        [HttpGet("GetAllCommentsByUser")]
        public async Task<IActionResult> GetAllCommetsByUserId([FromQuery] int pageNumber = 1, [FromQuery] bool isAssending = false)
        {

            var userEXist = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int result);


            User? user = null;
            if (userEXist)

                user = await _unitOfwork.User.GetById(result);

            if (user == null)
                return BadRequest("No such user");


           

            var comments = await _commentService.GetAllCommentInPage(x => x.UserId == result, nameof(Comment.CreatedAt), isAssending, pageNumber);
            


            var respoonse = new
            {
                commentResponse = comments.Items.Select(x => CommentResponseDTO.ConvertToCommentResponseDTO(x)).ToList(),
                pageNumber = comments.PageNumber,
                totalPages = comments.TotalPages,
                hasPrevious = comments.HasPrevious,
                hasNext = comments.HasNext,

            };
            return Ok(respoonse);

        }


        [HttpGet("GetAllCommentsByBlog/{id}")]
        public async Task<IActionResult> GetAllCommentsByBlog(int id, [FromQuery]int pageNumber=1,[FromQuery]bool isAssending=false)
        {

            Blog? blog = await _unitOfwork.Blog.GetById(id);
            if (blog == null)
                return NotFound("No such blog");

            var comments =await  _commentService.GetAllCommentInPage(x=>x.BlogId==id,nameof(Comment.CreatedAt), isAssending, pageNumber);
          


            var respoonse = new
            {
                commentResponse = comments.Items.Select(x => CommentResponseDTO.ConvertToCommentResponseDTO(x)).ToList(),
                pageNumber=comments.PageNumber,
                totalPages=comments.TotalPages,
                hasPrevious=comments.HasPrevious,
                hasNext=comments.HasNext,

            };
            return Ok(respoonse);
        }





        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment(CreateCommentDTO request)
        {
            if (ModelState.IsValid)
            {
                var userEXist = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int result);

               
                User? user = null;
                if (userEXist)
                
                    user = await _unitOfwork.User.GetById(result);
                
                if (user == null)
                    return BadRequest("No such user");



                Blog? blog = await _unitOfwork.Blog.GetById(request.BlogId);

                if (blog == null)
                    return BadRequest("the BlogId not Exist");

            

               

                Comment comment = CreateCommentDTO.ConvertToComment(request);
                comment.UserId = result;

                await _unitOfwork.Comment.Create(comment);
                await _unitOfwork.CompleteAsync();

                var commentResponse = CommentResponseDTO.ConvertToCommentResponseDTO(comment);

                return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, commentResponse);
            }

            var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
            return BadRequest(errors);
        }


        [HttpPut]
        [Authorize]
        public async Task<IActionResult>UpdateComment(Guid id ,UpdateCommentDTO request)
        {
            Comment? comment = await _unitOfwork.Comment.GetById(id,true);
            if (comment == null)
                return NotFound("No such comment");


            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (comment.UserId.ToString() != currentUserId)
            {
                return Forbid("You are not the author of this blog.");
            }


            _unitOfwork.Attach(comment);
            comment.Content = request.Content;
            _unitOfwork.MarkModified(comment, new[] { nameof(Comment.Content) });
            await _unitOfwork.CompleteAsync();

            var response=CommentResponseDTO.ConvertToCommentResponseDTO(comment);
            return Ok(response);
        }

        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> VoteOnComment(Guid commentId,VoteType vote)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User is not authenticated.");
            }

            Comment? comment = await _unitOfwork.Comment.GetById(x => x.Id == commentId, new[] { nameof(Comment.Votes) });
            if (comment == null)
                return NotFound("No such a comment");


           Vote ?voredExist= comment.Votes.FirstOrDefault(x => x.UserId.ToString() == currentUserId);

            if (voredExist != null)
            {
                return BadRequest("You have already voted on this comment");
            }
           


           bool result= int.TryParse(currentUserId, out int userId);
            if (!result)
            {
                return Problem("the auhanticated user has conflicted id");
            }
            var newVote = new Vote
            {
                CommentId = commentId,
                  UserId = userId,
                VoteType = vote
            };
          await   _unitOfwork.Vote.Create(newVote);
            await _unitOfwork.CompleteAsync();
            return Ok("Vote has been successfully cast.");
        }



        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {

            try
            {
                Comment? comment = await _unitOfwork.Comment.GetById(id, true);
                if (comment == null)
                    return NotFound("No such comment");

                 var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                 if (comment.UserId.ToString() != currentUserId)
                 {
                     return Forbid("You are not the author of this comment.");
                 }

                bool result = await _unitOfwork.Comment.Delete(id);
                if (!result)
                {
                    return StatusCode(500, "Error deleting comment.");
                }

                await _unitOfwork.CompleteAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        return BadRequest("Cannot delete comment due to foreign key constraint violation.");
                    }
                }

                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }
}
