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


        /// <summary>
        /// Retrieves a comment by its unique identifier.
        /// </summary>
        /// <param name="id">The GUID of the comment to retrieve.</param>
        /// <returns>
        /// Returns the comment details if found; otherwise, returns a not found response.
        /// </returns>
        /// <response code="200">Returns the comment details.</response>
        /// <response code="404">If no comment exists with the provided ID.</response>
        /// <remarks>
        /// Related entities included: Blog, User, and Votes.
        /// </remarks>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            Comment? comment = await _unitOfwork.Comment.GetById(x => x.Id==id, new[] {nameof(Comment.Blog),nameof(Comment.User),nameof(Comment.Votes)});
            if(comment == null)
                return NotFound("No such comment");

            var commentResponse=CommentResponseDTO.ConvertToCommentResponseDTO(comment);    

            return Ok(commentResponse);

        }


        /// <summary>
        /// Retrieves all comments created by the currently authenticated user, paginated.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="isAssending">Determines if the comments should be sorted in ascending order by creation date. Defaults to false (descending).</param>
        /// <returns>
        /// A paginated list of the user's comments along with pagination metadata.
        /// </returns>
        /// <response code="200">Returns the list of comments for the current user.</response>
        /// <response code="400">If the user is not authenticated or doesn't exist.</response>
        /// <remarks>
        /// Requires the user to be authenticated. Comments are returned with pagination details.
        /// </remarks>
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


        /// <summary>
        /// Retrieves all comments for a specific blog post, paginated.
        /// </summary>
        /// <param name="id">The ID of the blog post.</param>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="isAssending">Determines if the comments should be sorted in ascending order by creation date. Defaults to false (descending).</param>
        /// <returns>
        /// A paginated list of comments for the specified blog post.
        /// </returns>
        /// <response code="200">Returns the paginated list of comments.</response>
        /// <response code="404">If the blog post with the specified ID does not exist.</response>
        /// <remarks>
        /// Comments are sorted by creation date with pagination support.
        /// </remarks>
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



        /// <summary>
        /// Creates a new comment for a blog post.
        /// </summary>
        /// <param name="request">The comment creation request data.</param>
        /// <returns>
        /// Returns the created comment details if successful.
        /// </returns>
        /// <response code="201">Comment created successfully.</response>
        /// <response code="400">If the user does not exist, blog ID is invalid, or the request model is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <remarks>
        /// Requires authentication. Associates the comment with the authenticated user and the specified blog.
        /// </remarks>

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

        /// <summary>
        /// Updates the content of an existing comment.
        /// </summary>
        /// <param name="id">The unique identifier of the comment to update.</param>
        /// <param name="request">The updated comment data.</param>
        /// <returns>
        /// Returns the updated comment details if successful.
        /// </returns>
        /// <response code="200">Comment updated successfully.</response>
        /// <response code="403">If the authenticated user is not the author of the comment.</response>
        /// <response code="404">If the comment with the specified ID does not exist.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <remarks>
        /// Requires authentication. Only the author of the comment can update it.
        /// </remarks>
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



        /// <summary>
        /// Casts a vote on a specific comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to vote on.</param>
        /// <param name="vote">The type of vote (e.g., Upvote or Downvote).</param>
        /// <returns>
        /// Returns a confirmation message if the vote is successful.
        /// </returns>
        /// <response code="200">Vote successfully cast.</response>
        /// <response code="400">If the user has already voted on this comment.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the comment does not exist.</response>
        /// <remarks>
        /// Requires authentication. A user can only vote once on a given comment.
        /// </remarks>
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


        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the comment to delete.</param>
        /// <returns>
        /// Returns 204 No Content if deletion is successful.
        /// </returns>
        /// <response code="204">Comment deleted successfully.</response>
        /// <response code="400">If the comment cannot be deleted due to foreign key constraints.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not the author of the comment.</response>
        /// <response code="404">If the comment does not exist.</response>
        /// <response code="500">If an unexpected error occurs during deletion.</response>
        /// <remarks>
        /// Requires authentication. Only the author of the comment can delete it.
        /// </remarks>
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
