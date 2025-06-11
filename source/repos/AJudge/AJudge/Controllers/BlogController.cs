using AJudge.Application.DtO.BlogDTO;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using AJudge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace AJudge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        /// <summary>
        /// Retrieves a specific blog post by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the blog post.</param>
        /// <returns>
        /// Returns the blog details including author and votes if found; 
        /// otherwise, returns 404 with a "no such blog" message.
        /// </returns>
        /// <response code="200">Returns the blog details</response>
        /// <response code="404">If no blog with the given ID is found</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            Blog? blog = await _unitOfWork.Blog.GetBlogById(id, new[] { "Author", "Votes" });
            if (blog == null)
                return NotFound("no such blog");

            var response = BlogDetailsDTO.ConvertToBlogDTO(blog);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves all blog posts including their authors and votes.
        /// </summary>
        /// <returns>
        /// A list of all blog posts, each with author and vote details.
        /// </returns>
        /// <response code="200">Returns the list of blog posts</response>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _unitOfWork.Blog.GetAllBlogs(new[] { "Author", "Votes" });

            var responses = blogs?.Select(x => BlogDetailsDTO.ConvertToBlogDTO(x)).ToList();
            return Ok(responses);
        }

        /// <summary>
        /// Creates a new blog post.
        /// </summary>
        /// <param name="request">The blog data to create, including title </param>
        /// <returns>
        /// Returns the created blogID and the content and the time that created at it post data with a 201 status code if successful; otherwise, returns a 400 error.
        /// </returns>
        /// <response code="201">Returns the newly created blog</response>
        /// <response code="400">If the user doesn't exist or model validation fails</response>
        /// <remarks>
        /// Requires user to be authenticated (Bearer token).
        /// </remarks>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBlog(CreateBlogDTo request)
        {

             var userEXist = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value,out int result);

            User? user=null;
            if (userEXist)
            {
                user = await _unitOfWork.User.GetById(result);
            }
            if (user == null)
                return BadRequest("No such user");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            Blog blog = CreateBlogDTo.ConvertToBlog(request);
            blog.AuthorUserId = result;
            await _unitOfWork.Blog.Create(blog);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(GetBlogById), new { id = blog.BlogId }, request);

        }


        /// <summary>
        /// Updates the content of an existing blog post.
        /// </summary>
        /// <param name="id">The ID of the blog post to update.</param>
        /// <param name="request">The updated blog content.</param>
        /// <returns>
        /// Returns the updated blog data if the update is successful; otherwise, returns an appropriate error response.
        /// </returns>
        /// <response code="200">Returns the updated blog post</response>
        /// <response code="400">If the request model is invalid</response>
        /// <response code="403">If the current user is not the author of the blog</response>
        /// <response code="404">If the blog post does not exist</response>
        /// <remarks>
        /// Requires authentication (Bearer token). Only the blog's author can update it.
        /// </remarks>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateBlog(int id,UpdateBlogDTO request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            Blog? blog = await _unitOfWork.Blog.GetBlogById(id, new[] { "Author", "Votes" });
            if (blog == null)
                return NotFound("No such blog");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (blog.AuthorUserId.ToString() != currentUserId)
            {
                return StatusCode(403,new { message="You are not the author of this blog" });
            } 


            _unitOfWork.Attach(blog);
            blog.Content = request.Content;
            _unitOfWork.MarkModified(blog, new[] { nameof(Blog.Content) });
           await  _unitOfWork.CompleteAsync();

            var response = BlogDetailsDTO.ConvertToBlogDTO(blog);

            return Ok(response);

        }

        /// <summary>
        /// add a vote (like or dislike) on a blog post by the authenticated user.
        /// </summary>
        /// <param name="blogID">The ID of the blog to vote on</param>
        /// <param name="vote">The type of vote (Like or Dislike)</param>
        /// <returns>
        /// Returns:
        /// - <c>200 OK</c> if the vote is successfully added.
        /// - <c>400 Bad Request</c> if the user has already voted on the blog.
        /// - <c>401 Unauthorized</c> if the user is not authenticated.
        /// - <c>404 Not Found</c> if the blog does not exist.
        /// - <c>500 Internal Server Error</c> for unexpected issues.
        /// </returns>
        /// <remarks>
        /// This endpoint is protected and requires the user to be authenticated.
        /// Users can only vote once per blog. If a user tries to vote again, they will receive an error.
        /// </remarks>
        [HttpPost("VoteBlog")]
        [Authorize]
        public async Task<IActionResult> VoteBlog(int blogID, VoteType vote)
        {
            /*
            var  userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Blog? blog = await _unitOfWork.Blog.GetById(BlogID);
            if (blog == null)
                return NotFound("no such blog");
            var exist= blog.Votes.FirstOrDefault(x => x.UserId.ToString() == userId && x.BlogId == BlogID);
            if (exist != null)
            {
                if (true)
                {

                    return BadRequest("You have already voted this blog with the same vote type.");


                }

                exist.VoteType = (VoteType)vote;
                _unitOfWork.MarkModified(exist, new[] { nameof(Vote.VoteType) });
               // await _unitOfWork.CompleteAsync();
                return Ok("Vote updated successfully.");
            }
            else
            {
                Vote newVote = new Vote
                {
                    UserId = userdid,
                    BlogId = BlogID,
                    VoteType = (VoteType)vote
                };
                await _unitOfWork.Vote.Create(newVote);
            }
            //await _unitOfWork.CompleteAsync();

            return Ok("Vote added successfully.");
            */

            if (!Enum.IsDefined(typeof(VoteType), vote))
                return BadRequest("Invalid vote type.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }
            Blog? blog = await _unitOfWork.Blog.GetById(x => x.BlogId == blogID, new[] { nameof(Blog.Votes) });
            if (blog == null)
                return NotFound("No such a blog");

            Vote? voteExist = blog.Votes.FirstOrDefault(x => x.UserId.ToString() == userId);
            if (voteExist != null)
            {
                return BadRequest("You have already voted on this blog");
            }

            bool result = int.TryParse(userId, out int usId);
            if (!result)
            {
                return Problem("the auhanticated user has conflicted id");
            }
            var newVote = new Vote
            {
                BlogId = blogID,
                UserId = usId,
                VoteType = vote
            };

            await _unitOfWork.Vote.Create(newVote);
            await _unitOfWork.CompleteAsync();
            return Ok("Vote has been successfully added");

        }



        /// <summary>
        /// Deletes a blog post by its ID.
        /// </summary>
        /// <param name="id">The ID of the blog to delete.</param>
        /// <returns>
        /// Returns <c>NoContent</c> if the deletion is successful; otherwise returns appropriate error responses.
        /// </returns>
        /// <response code="204">Blog deleted successfully</response>
        /// <response code="403">If the current user is not the author of the blog</response>
        /// <response code="404">If the blog post does not exist</response>
        /// <remarks>
        /// Requires authentication. Only the author of the blog can delete it.
        /// </remarks>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlog(int id)
        {

            Blog? blog = await _unitOfWork.Blog.GetById(id);
            if (blog == null)
                return NotFound("no such  blog");

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (blog.AuthorUserId.ToString() != currentUserId)
            {
                return Forbid("You are not the author of this blog.");
            }

            await  _unitOfWork.Blog.Delete(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();

        }
    }
}
