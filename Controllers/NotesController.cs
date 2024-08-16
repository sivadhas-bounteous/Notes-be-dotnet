using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Data;
using Notes.DTO;
using Notes.Models;
using System.Security.Claims;


namespace Notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public NotesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/notes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteResponse>>> GetAllNotes()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var notesQuery = isAdmin ? _context.Notes : _context.Notes.Where(note => note.UserId == currentUserId);

            var notes = await notesQuery.ToListAsync();

            var noteResponses = _mapper.Map<IEnumerable<NoteResponse>>(notes);

            return Ok(noteResponses);
        }

        // GET: api/notes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NoteResponse>> GetNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (note.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var noteResponse = _mapper.Map<NoteResponse>(note);

            return Ok(noteResponse);
        }

        // POST: api/notes
        [HttpPost]
        public async Task<ActionResult<NoteResponse>> PostNote([FromBody] NoteInput noteInput)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var note = _mapper.Map<Note>(noteInput);
            note.UserId = userId; // Set the UserId for the note
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = DateTime.UtcNow;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            var noteResponse = _mapper.Map<NoteResponse>(note);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, noteResponse);
        }

        // PUT: api/notes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> EditNote(int id, [FromBody] NoteInput noteInput)
        {
            var existingNote = await _context.Notes.FindAsync(id);
            if (existingNote == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (existingNote.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            existingNote.Title = noteInput.Title;
            existingNote.Content = noteInput.Content;
            existingNote.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/notes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (note.UserId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
