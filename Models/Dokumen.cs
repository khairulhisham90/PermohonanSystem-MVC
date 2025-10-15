namespace PermohonanSystemMVC.Models
{
    public class Dokumen
    {
        public int Id { get; set; }
        public int PermohonanId { get; set; }
        public Permohonan Permohonan { get; set; }

        public string NamaFail { get; set; }
        public string PathFail { get; set; }
        public long SaizByte { get; set; }
    }
}
