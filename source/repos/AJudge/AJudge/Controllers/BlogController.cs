using AJudge.Application.DtO.BlogDTO;
using AJudge.Domain.Entities;
using AJudge.Domain.RepoContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                return Forbid("You are not the author of this blog.");
            } 


            _unitOfWork.Attach(blog);
            blog.Content = request.Content;
            _unitOfWork.MarkModified(blog, new[] { nameof(Blog.Content) });
           await  _unitOfWork.CompleteAsync();

            var response = BlogDetailsDTO.ConvertToBlogDTO(blog);

            return Ok(response);

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
