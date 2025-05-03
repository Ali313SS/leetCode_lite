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


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            Blog? blog = await _unitOfWork.Blog.GetBlogById(id, new[] { "Author", "Votes" });
            if (blog == null)
                return NotFound("no such blog");

            var response = BlogDetailsDTO.ConvertToBlogDTO(blog);
            return Ok(response);
        }

        [HttpGet]

        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _unitOfWork.Blog.GetAllBlogs(new[] { "Author", "Votes" });

            var responses = blogs?.Select(x => BlogDetailsDTO.ConvertToBlogDTO(x)).ToList();
            return Ok(responses);
        }


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
