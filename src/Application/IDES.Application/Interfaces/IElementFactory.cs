using IDES.Domain.Catalogue;
using IDES.Domain.Metre;

namespace IDES.Application.Interfaces;

public interface IElementFactory
{
    ElementDynamique CreerElementDynamique(DefinitionElement definition);
}
