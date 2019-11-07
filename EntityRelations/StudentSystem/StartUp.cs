using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem 
{
    public class StartUp
    {
        public static void Main()
        {
            SeedStudentData seed = new SeedStudentData();

            seed.Seed(new StudentSystemContext());

            using (var db = new StudentSystemContext())
            {
                db.Database.Migrate();
            }
        }
    }
}
