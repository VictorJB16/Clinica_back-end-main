namespace Clinica_back_end.DTO.Cita
{
    public class CreateCitaDTO
    {
        public DateTime FechaHora { get; set; }
        public int SucursalId { get; set; }
        public int TipoCitaId { get; set; }
    }
}
