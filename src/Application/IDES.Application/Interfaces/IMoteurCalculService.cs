using IDES.Domain.Metre;

namespace IDES.Application.Interfaces;

public interface IMoteurCalculService
{
    object? Evaluer(string? formule, ElementDynamique element);
}
