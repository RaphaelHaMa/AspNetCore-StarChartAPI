using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObject == null) return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObject.Id == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);
            if (!celestialObjects.Any()) return NotFound();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObject.Name == name).ToList();
            }
            return Ok(celestialObjects.ToList());
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { Id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject cObject)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (celestialObject == null) return NotFound();

            celestialObject.Name = cObject.Name;
            celestialObject.OrbitalPeriod = cObject.OrbitalPeriod;
            celestialObject.OrbitedObjectId = cObject.OrbitedObjectId;
            _context.CelestialObjects.Update(celestialObject);
            _context.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var cObject = _context.CelestialObjects.FirstOrDefault(x => x.Id == id);
            if (cObject == null) return NotFound();

            cObject.Name = name;
            _context.CelestialObjects.Update(cObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (!celestialObjects.Any()) return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
