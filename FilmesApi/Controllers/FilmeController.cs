using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmeController : ControllerBase
    {
        private IMapper _mapper;
        private FilmeContext _context;

        public FilmeController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adiciona um filme ao banco de dados
        /// </summary>
        /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
        /// <returns>IActionResult</returns>
        /// <response code="201">Caso inserção seja feita com sucesso</response>

        [HttpPost] // info origem -> corpo da requisição
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
        {
            Filme filme = _mapper.Map<Filme>(filmeDto); // conversão filme dto em filme
            _context.Filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme); // post -> chama o get, passando id e o filme adicionado!
        }


        [HttpGet]
        public IEnumerable<ReadFilmeDto> RetornarFilmes([FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));  // skip e take -> paginação, origem rota após "?"
        }


        [HttpGet("{id}")] // info origem - parametro da rota após "/"
        public IActionResult RecuperaFilmePorId(int id)
        {
            Filme filmeEncontrado = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filmeEncontrado == null) return NotFound();
            var filmeDto = _mapper.Map<ReadFilmeDto>(filmeEncontrado);
            return Ok(filmeDto);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null) return NotFound();
            _mapper.Map(filmeDto, filme); // atualiza file a partir de filme dto
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch]
        public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null) return NotFound();
            var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
            patch.ApplyTo(filmeParaAtualizar, ModelState); // verifica se é possível aplicar a parte atualizada no filme do banco que convertemos.

            if (!TryValidateModel(filmeParaAtualizar))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();

        }


        [HttpDelete("{id}")]
        public IActionResult DeletaFilme(int id)
        {
            var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if (filme == null) return NotFound();
            _context.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
