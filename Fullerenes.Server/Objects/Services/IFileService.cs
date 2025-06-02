using System.Numerics;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using Microsoft.AspNetCore.Mvc;
using static Fullerenes.Server.Objects.Services.ServicesImpl.FileService;

namespace Fullerenes.Server.Objects.Services
{
    public interface IFileService
    {
        string Write(IReadOnlyCollection<LimitedArea> areas, string fileName, string? subFolder = null);
        AreaMainInfo ReadMainInfo(string fullPath);
        LimitedArea GetArea(string fullPath);
        LimitedArea[] GetAreas(long genId);
    }
}
