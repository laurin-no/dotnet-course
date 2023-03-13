using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnivEnrollerApi.Data;
using UnivEnrollerApi.Models;

namespace UnivEnrollerApi.Controllers;

[Route("")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly UnivEnrollerContext _context;

    public CoursesController(UnivEnrollerContext context)
    {
        _context = context;
    }

    // // GET: api/University
    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<University>>> GetUniversities()
    // {
    //     if (_context.Universities == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return await _context.Universities.ToListAsync();
    // }

    // // GET: api/University/5
    // [HttpGet("{id}")]
    // public async Task<ActionResult<University>> GetUniversity(int id)
    // {
    //     if (_context.Universities == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     var university = await _context.Universities.FindAsync(id);
    //
    //     if (university == null)
    //     {
    //         return NotFound();
    //     }
    //
    //     return university;
    // }

    [HttpGet("/university/{id}/courses")]
    public async Task<ActionResult<List<Course>>> GetUniversityCourses(int id)
    {
        var courses = await _context.Cources.Where(c => c.UniversityId == id).ToListAsync();

        return courses;
    }

    [HttpGet("/student/{id}/courses")]
    public async Task<ActionResult<List<Enrollment>>> GetStudentCourses(int id)
    {
        var courses = await _context.Enrollments.Where(e => e.StudentId == id).ToListAsync();

        return courses;
    }

    [HttpPut("/grade")]
    public async Task<IActionResult> PutUniversity(GradeModel grade)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == grade.StudentId && e.CourseId == grade.CourseId);

        if (enrollment == null)
        {
            return NotFound();
        }

        enrollment.Grade = grade.Grade;
        enrollment.GradingDate = grade.GradingDate;

        _context.Entry(enrollment).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpPost("/course")]
    public async Task<ActionResult<Course>> PostCourse(CourseModel course)
    {
        var id = _context.Cources.Max(i => i.Id) + 1;
        var c = new Course
        {
            Id = id,
            Name = course.Name,
            UniversityId = course.UniversityId
        };
        _context.Cources.Add(c);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCourse", new { id = c.Id }, c);
    }

    [HttpGet("/course/{id}")]
    public async Task<ActionResult<Course>> GetCourse(int id)
    {
        var course = await _context.Cources.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return course;
    }

    [HttpDelete("/student/{studentId}/course/{courseId}")]
    public async Task<IActionResult> DeleteEnrollment(int studentId, int courseId)
    {
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Grade == null);

        if (enrollment == null)
        {
            return NoContent();
        }

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UniversityExists(int id)
    {
        return (_context.Universities?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}