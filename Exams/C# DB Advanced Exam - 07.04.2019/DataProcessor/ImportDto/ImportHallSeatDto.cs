using System;
namespace Cinema.DataProcessor.ImportDto
{
    public class ImportHallSeatDto
    {
        public string Name { get; set; }

        public bool Is4Dx { get; set; }

        public bool Is3D { get; set; }

        public int Seats { get; set; }
    }
}
