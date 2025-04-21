namespace Clinica_back_end.DTO.Cita
{
    public class UpdateCitaDTO
    {
        public DateTime FechaHora { get; set; }
        public string Lugar { get; set; } = null!;
        public string Status { get; set; } = null!;// ACTIVA, CANCELADA
        public int PacienteId { get; set; }
        public int SucursalId { get; set; }
        public int TipoCitaId { get; set; }
    }
}
