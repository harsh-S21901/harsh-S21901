using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TestPractice3.DTOs;

namespace TestPractice3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly string connectionstring = "Data Source=(localdb)\\Local;Integrated Security=True";

        [HttpGet]

        public async Task<IActionResult> GetAritstInfo(int Id)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                var info = new Artist_DTO();
                var organiser = new List<Organiser_DTO>();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    await connection.OpenAsync();
                    command.CommandText = $"select o.IdOrganiser , o.Name from Organiser o join Event_Organiser e on o.IdOrganiser = e.IdOrganiser " +
                        $" join Event ev on ev.IdEvent = e.IdEvent " +
                        $" join Artist_Event ae on ae.IdEvent = ev.IdEvent " +
                        $" where ae.IdArtist = {Id}; ";
                    var reader = await command.ExecuteReaderAsync();
                    if (!reader.HasRows) return BadRequest("Artist doesnt exist!");
                    while (await reader.ReadAsync())
                    {
                        organiser.Add(new Organiser_DTO
                        {
                            IdOrganiser = int.Parse(reader["IdOrganiser"].ToString()),
                            Name = reader["Name"].ToString(),
                        });
                    }
                    await reader.CloseAsync();
                    await command.ExecuteNonQueryAsync();

                    command.CommandText = $"Select a.IdArtist, a.Nickname, e.PerformanceDate from Artist a " +
                        $"join Artist_Event e on a.IdArtist =e.IdArtist where a.IdArtist ={Id}";

                    reader = await command.ExecuteReaderAsync();
                    if (!reader.HasRows) return NotFound();
                    while (await reader.ReadAsync())
                    {
                        info = new Artist_DTO
                        {
                            IdArtist = int.Parse(reader["IdArtist"].ToString()),
                            Nickname = reader["IdArtist"].ToString(),
                            Organisers = organiser
                        };
                    }

                    await reader.CloseAsync();
                    await command.ExecuteNonQueryAsync();
                    await connection.CloseAsync();
                }
                return Ok(info);
            }


        }

        [HttpPut]
        public async Task<IActionResult> updateDate(int IdArtist,int IdEvent, DateTime date)
        {
            using (var connection = new SqlConnection(connectionstring))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    await connection.OpenAsync();
                    command.CommandText = $"select  e.StartDate, e.EndDate from Artist_Event a join Event e on a.IdEvent = e.IdEvent where a.IdArtist ={IdArtist} and e.IdEvent={IdEvent}";
                    var reader = await command.ExecuteReaderAsync();
                     await reader.ReadAsync();
                    
                    var Startdate = DateTime.Parse(reader["StartDate"].ToString());
                    var Enddate = DateTime.Parse(reader["EndDate"].ToString());
                     await reader.CloseAsync();
                    await command.ExecuteNonQueryAsync();
                    if (Startdate < DateTime.Now)
                    {
                        return BadRequest("Event has already passed!");
                    }
                    if (Enddate > date && date > Startdate)
                    {
                        command.CommandText = $"update Artist_Event set PerformanceDate = {date} where IdArtist ={IdArtist} and IdEvent ={IdEvent}";
                        await command.ExecuteNonQueryAsync();
                        
                    }
                    await reader.CloseAsync();
                    await connection.CloseAsync();
                    return Ok("upgraded");
                }

            }
        }
    }
}
