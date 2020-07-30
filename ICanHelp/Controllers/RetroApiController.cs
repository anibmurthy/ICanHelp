using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHelp.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ICanHelp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetroApiController : ControllerBase
    {
        private IRetrospectiveRepository _retroRepo;

        public RetroApiController(IRetrospectiveRepository retroRepo)
        {
            _retroRepo = retroRepo;
        }

        [HttpGet]
        [Route("AddComment/{boardId}")]
        public async Task<IActionResult> AddComment(int boardId, int column, string comment)
        {
            if (column > 4)
            {
                return BadRequest("Invalid column number!");
            }

            if (boardId < 1 || !await _retroRepo.IsBoardExists(boardId))
            {
                return BadRequest("Invalid board!");
            }
            string clientId = Request.Headers["x-conn-id"];

            int id = await _retroRepo.AddComment(clientId, boardId, column, comment);

            if (id == 0)
                return new ObjectResult($"Could not add comment: {comment}") { StatusCode = 500 };
            else
                return Ok(id);
        }

        [HttpGet]
        [Route("DeleteComment/{boardId}")]
        public async Task<IActionResult> DeleteComment(int boardId, int column, int commentId)
        {
            if (column > 4)
            {
                return BadRequest("Invalid column number!");
            }

            if (boardId < 1 || !await _retroRepo.IsBoardExists(boardId))
            {
                return BadRequest("Invalid board!");
            }
            string clientId = Request.Headers["x-conn-id"];

            bool result = await _retroRepo.DeleteComment(clientId, boardId, column, commentId);

            if (!result)
                return new ObjectResult($"Could not delete comment: {commentId}") { StatusCode = 500 };
            else
                return Ok($"Deleted {commentId}");
        }

        [HttpGet]
        [Route("EditComment/{boardId}")]
        public async Task<IActionResult> EditComment(int boardId, int column, int commentId, string comment)
        {
            if (column > 4)
            {
                return BadRequest("Invalid column number!");
            }

            if (boardId < 1 || !await _retroRepo.IsBoardExists(boardId))
            {
                return BadRequest("Invalid board!");
            }
            string clientId = Request.Headers["x-conn-id"];

            bool result = await _retroRepo.EditComment(clientId, boardId, column, commentId, comment);

            if (!result)
                return new ObjectResult($"Could not edit comment: {commentId}") { StatusCode = 500 };
            else
                return Ok($"Edited comment: {commentId}");
        }

        [HttpGet]
        [Route("Copy/{boardId}")]
        public async Task<IActionResult> Copy(int boardId)
        {
            string result = await _retroRepo.CopyBoard(boardId);

            if (string.IsNullOrWhiteSpace(result))
                return BadRequest("Nothing to Copy");

            return Ok(result);
        }

    }
}