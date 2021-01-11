﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;
using CoffeeShop.Data;
using System.Linq;

namespace CoffeeShop.Repositories
{
    public class BeanVarietyRepository : IBeanVarietyRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;
        public BeanVarietyRepository(IConfiguration configuration, ApplicationDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }
        public List<BeanVariety> GetAll()
        {
            return _context.BeanVariety.ToList();
        }
        //public List<BeanVariety> GetAll()
        //{
        //    using (var conn = Connection)
        //    {
        //        conn.Open();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "SELECT Id, [Name], Region, Notes FROM BeanVariety;";
        //            var reader = cmd.ExecuteReader();
        //            var varieties = new List<BeanVariety>();
        //            while (reader.Read())
        //            {
        //                var variety = new BeanVariety()
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Name = reader.GetString(reader.GetOrdinal("Name")),
        //                    Region = reader.GetString(reader.GetOrdinal("Region")),
        //                };
        //                if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
        //                {
        //                    variety.Notes = reader.GetString(reader.GetOrdinal("Notes"));
        //                }
        //                varieties.Add(variety);
        //            }

        //            reader.Close();

        //            return varieties;
        //        }
        //    }
        //}
        public BeanVariety Get(int id)
        {
            return _context.BeanVariety.FirstOrDefault(bv => bv.Id == id);
        }

        /*public BeanVariety Get(int id)
        {
            return _context.BeanVariety.FirstOrDefault(bv => bv.Id == id);
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Region, Notes 
                          FROM BeanVariety
                         WHERE Id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    BeanVariety variety = null;
                    if (reader.Read())
                    {
                        variety = new BeanVariety()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Region = reader.GetString(reader.GetOrdinal("Region")),
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Notes")))
                        {
                            variety.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                        }
                    }

                    reader.Close();

                    return variety;
                }
            }
        }*/

        public void Add(BeanVariety variety)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO BeanVariety ([Name], Region, Notes)
                        OUTPUT INSERTED.ID
                        VALUES (@name, @region, @notes)";
                    cmd.Parameters.AddWithValue("@name", variety.Name);
                    cmd.Parameters.AddWithValue("@region", variety.Region);
                    if (variety.Notes == null)
                    {
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@notes", variety.Name);
                    }

                    variety.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(BeanVariety variety)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE BeanVariety 
                           SET [Name] = @name, 
                               Region = @region, 
                               Notes = @notes
                         WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", variety.Id);
                    cmd.Parameters.AddWithValue("@name", variety.Name);
                    cmd.Parameters.AddWithValue("@region", variety.Region);
                    if (variety.Notes == null)
                    {
                        cmd.Parameters.AddWithValue("@notes", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@notes", variety.Name);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM BeanVariety WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}