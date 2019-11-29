namespace Cinema.DataProcessor.ExportDto
{
    public class ExportMovieDto
    {
        public string MovieName { get; set; }

        public string Rating { get; set; }

        public string TotalIncomes { get; set; }

        public ExportCustomerDto[] Customers { get; set; }
    }
}
