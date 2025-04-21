using AutoMapper;
using Clinica_back_end.DTO.Cita;
using Clinica_back_end.DTO.Paciente;
using Clinica_back_end.DTO.Sucursal;
using Clinica_back_end.DTO.TipoCita;
using Clinica_back_end.Entities;

namespace Clinica_back_end.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Cita, CitaDTO>().ReverseMap();
            CreateMap<Cita, CreateCitaDTO>().ReverseMap();
            CreateMap<Cita, UpdateCitaDTO>().ReverseMap();

            CreateMap<Paciente, PacienteDTO>().ReverseMap();
            CreateMap<Paciente, CreatePacienteDTO>().ReverseMap();
            CreateMap<Paciente, UpdatePacienteDTO>().ReverseMap();

            CreateMap<Sucursal, SucursalDTO>().ReverseMap();
            CreateMap<Sucursal, CreateSucursalDTO>().ReverseMap();
            CreateMap<Sucursal, UpdateSucursalDTO>().ReverseMap();

            CreateMap<TipoCita, TipoCitaDTO>().ReverseMap();
            CreateMap<TipoCita, CreateTipoCitaDTO>().ReverseMap();
            CreateMap<TipoCita, UpdateTipoCitaDTO>().ReverseMap();
        }
    }
}
