using AutoMapper;
using PetCore.API.DTOs.Appointments;
using PetCore.API.DTOs.Auth;
using PetCore.API.DTOs.Clinics;
using PetCore.API.DTOs.CostCenters;
using PetCore.API.DTOs.Exams;
using PetCore.API.DTOs.Financial;
using PetCore.API.DTOs.Hospitalizations;
using PetCore.API.DTOs.MedicalRecords;
using PetCore.API.DTOs.Movements;
using PetCore.API.DTOs.Orders;
using PetCore.API.DTOs.Patients;
using PetCore.API.DTOs.Products;
using PetCore.API.DTOs.Species;
using PetCore.API.DTOs.Tutors;
using PetCore.API.DTOs.Users;
using PetCore.Domain.Entities;

namespace PetCore.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Clinic
        CreateMap<Clinic, ClinicDto>();
        CreateMap<CreateClinicRequest, Clinic>();
        CreateMap<UpdateClinicRequest, Clinic>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.Clinics, opt => opt.MapFrom(s =>
                s.ClinicUsers.Select(cu => new UserClinicDto
                {
                    ClinicId = cu.ClinicId,
                    ClinicName = cu.Clinic.Name,
                    Role = cu.Role.ToString()
                }).ToList()));

        CreateMap<User, UserInfoDto>();
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ClinicUser
        CreateMap<ClinicUser, ClinicUserDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.Name))
            .ForMember(d => d.UserEmail, opt => opt.MapFrom(s => s.User.Email))
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

        CreateMap<ClinicUser, ClinicInfoDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ClinicId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Clinic.Name))
            .ForMember(d => d.TradeName, opt => opt.MapFrom(s => s.Clinic.TradeName))
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));

        // Species & Breed
        CreateMap<Domain.Entities.Species, SpeciesDto>();
        CreateMap<Breed, BreedDto>();

        // Tutor
        CreateMap<Tutor, TutorDto>()
            .ForMember(d => d.PatientCount, opt => opt.MapFrom(s => s.Patients.Count));
        CreateMap<Tutor, TutorDetailDto>()
            .ForMember(d => d.PatientCount, opt => opt.MapFrom(s => s.Patients.Count))
            .ForMember(d => d.Patients, opt => opt.MapFrom(s => s.Patients));
        CreateMap<Patient, TutorPatientDto>()
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Species.Name))
            .ForMember(d => d.BreedName, opt => opt.MapFrom(s => s.Breed != null ? s.Breed.Name : null));
        CreateMap<CreateTutorRequest, Tutor>();
        CreateMap<UpdateTutorRequest, Tutor>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Patient
        CreateMap<Patient, PatientDto>()
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Tutor.Name))
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Species.Name))
            .ForMember(d => d.BreedName, opt => opt.MapFrom(s => s.Breed != null ? s.Breed.Name : null))
            .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.Sex.ToString()));
        CreateMap<Patient, PatientDetailDto>()
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Tutor.Name))
            .ForMember(d => d.TutorPhone, opt => opt.MapFrom(s => s.Tutor.Phone))
            .ForMember(d => d.TutorEmail, opt => opt.MapFrom(s => s.Tutor.Email))
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Species.Name))
            .ForMember(d => d.BreedName, opt => opt.MapFrom(s => s.Breed != null ? s.Breed.Name : null))
            .ForMember(d => d.Sex, opt => opt.MapFrom(s => s.Sex.ToString()));

        // Appointment
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Patient.Tutor.Name))
            .ForMember(d => d.TutorPhone, opt => opt.MapFrom(s => s.Patient.Tutor.Phone))
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Patient.Species.Name))
            .ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian != null ? s.Veterinarian.Name : null))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Appointment, CalendarEventDto>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s =>
                $"{s.Patient.Name} - {s.Type}"))
            .ForMember(d => d.Start, opt => opt.MapFrom(s => s.ScheduledAt))
            .ForMember(d => d.End, opt => opt.MapFrom(s => s.ScheduledAt.AddMinutes(s.DurationMinutes)))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // MedicalRecord
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian.Name));
        CreateMap<Prescription, PrescriptionDto>();

        // Hospitalization
        CreateMap<Hospitalization, HospitalizationDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Patient.Tutor.Name))
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Patient.Species.Name))
            .ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian.Name))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EvolutionCount, opt => opt.MapFrom(s => s.Evolutions.Count));
        CreateMap<Hospitalization, HospitalizationDetailDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Patient.Tutor.Name))
            .ForMember(d => d.SpeciesName, opt => opt.MapFrom(s => s.Patient.Species.Name))
            .ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian.Name))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EvolutionCount, opt => opt.MapFrom(s => s.Evolutions.Count));
        CreateMap<HospitalizationEvolution, EvolutionDto>()
            .ForMember(d => d.VeterinarianName, opt => opt.MapFrom(s => s.Veterinarian.Name));

        // Exam
        CreateMap<ExamType, ExamTypeDto>();
        CreateMap<ExamRequest, ExamRequestDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Patient.Tutor.Name))
            .ForMember(d => d.RequestedByName, opt => opt.MapFrom(s => s.RequestedBy.Name))
            .ForMember(d => d.ExamTypeName, opt => opt.MapFrom(s => s.ExamType.Name))
            .ForMember(d => d.ExamTypeCategory, opt => opt.MapFrom(s => s.ExamType.Category))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<ExamResult, ExamResultDto>()
            .ForMember(d => d.PerformedByName, opt => opt.MapFrom(s => s.PerformedBy.Name));

        // Product
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductUnit, ProductUnitDto>();
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.CategoryColor, opt => opt.MapFrom(s => s.Category.Color))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.Unit.Abbreviation));

        // Movement
        CreateMap<Movement, MovementDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.CreatedByName, opt => opt.MapFrom(s => s.CreatedBy.Name))
            .ForMember(d => d.ApprovedByName, opt => opt.MapFrom(s => s.ApprovedBy != null ? s.ApprovedBy.Name : null))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        // Order
        CreateMap<Order, OrderDto>()
            .ForMember(d => d.CreatedByName, opt => opt.MapFrom(s => s.CreatedBy.Name))
            .ForMember(d => d.ApprovedByName, opt => opt.MapFrom(s => s.ApprovedBy != null ? s.ApprovedBy.Name : null))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.UnitAbbreviation, opt => opt.MapFrom(s => s.Product.Unit.Abbreviation));

        // Financial
        CreateMap<FinancialCategory, FinancialCategoryDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
        CreateMap<FinancialTransaction, TransactionDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.FinancialCategoryName, opt => opt.MapFrom(s => s.FinancialCategory.Name))
            .ForMember(d => d.TutorName, opt => opt.MapFrom(s => s.Tutor != null ? s.Tutor.Name : null))
            .ForMember(d => d.CostCenterName, opt => opt.MapFrom(s => s.CostCenter != null ? s.CostCenter.Name : null))
            .ForMember(d => d.CreatedByName, opt => opt.MapFrom(s => s.CreatedBy.Name))
            .ForMember(d => d.PaymentMethod, opt => opt.MapFrom(s => s.PaymentMethod != null ? s.PaymentMethod.ToString() : null));
        CreateMap<TransactionInstallment, InstallmentDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        // CostCenter
        CreateMap<CostCenter, CostCenterDto>();
    }
}
