using Fullerenes.Server.Objects.CustomStructures;
using Fullerenes.Server.Objects.Enums;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Objects.LimitedAreas;
using System.Globalization;
using System.Numerics;
using static Fullerenes.Server.Objects.Services.ServicesImpl.FileService;
using static Fullerenes.Server.Objects.Services.ServicesImpl.FileService.AreaMainInfo;

namespace Fullerenes.Server.Objects.Services.ServicesImpl
{
    public static class Adapter
    {
        public static ICollection<FlatArea> AdaptToFlatArea(LimitedArea area)
            => area.Fullerenes
                .Select(fullerene => 
                    new FlatArea {
                        SeriesA = area.Series,
                        CenterA = area.Center.ToString(),
                        ParamsA = string.Join(", ", area.Params),
                        SizeF = fullerene.Size,
                        CenterF = fullerene.Center.ToString(),
                        EulerAnglesF = fullerene.EulerAngles.ToString(),
                    })
                .ToArray();

        public static AreaMainInfo AdaptToMainInfo(IReadOnlyCollection<FlatArea> flatArea)
        {
            int seriesA = flatArea.First().SeriesA;

            var flatCenterA = flatArea.First().CenterA;

            // Убираем угловые скобки
            flatCenterA = flatCenterA.Trim('<', '>').Replace(',', '.');

            // Разделяем строку на компоненты по пробелу
            string[] components = flatCenterA.Split(' ');

            // Преобразуем компоненты в float и создаем Vector3
            float xA = float.Parse(components[0].Trim(), CultureInfo.InvariantCulture);
            float yA = float.Parse(components[1].Trim(), CultureInfo.InvariantCulture);
            float zA = float.Parse(components[2].Trim(), CultureInfo.InvariantCulture);

            Vector3 centerA = new(xA, yA, zA);

            string flatParams = flatArea.First().ParamsA;

            // Разделяем строку по "), (" и убираем скобки
            string[] pairs = flatParams.Trim('(', ')').Split("), (", StringSplitOptions.None);

            // Создаем массив кортежей (string, float)
            var paramsA = pairs.Select(pair =>
            {
                // Разделяем пару на имя и значение
                var parts = pair.Split(", ");
                string name = parts[0];
                float value = float.Parse(parts[1]);

                return (name, value);
            }).ToArray();

            List<FullereneMainInfo> fullerenesA = new List<FullereneMainInfo>(flatArea.Count());

            foreach (FlatArea record in flatArea)
            {
                float sizeF = record.SizeF;

                var flatCenterF = record.CenterF;

                // Убираем угловые скобки
                flatCenterF = flatCenterF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                string[] componentsF = flatCenterF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float xF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float yF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float zF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                Vector3 centerF = new(xF, yF, zF);

                var flatEulerAnglesF = record.EulerAnglesF;

                // Убираем угловые скобки
                flatEulerAnglesF = flatEulerAnglesF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                componentsF = flatEulerAnglesF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float aF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float bF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float gF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                EulerAngles eulerAnglesF = new()
                {
                    PraecessioAngle = aF,
                    NutatioAngle = bF,
                    ProperRotationAngle = gF,
                };

                fullerenesA.Add(new FullereneMainInfo
                {
                    Size = sizeF,
                    Center = centerF,
                    EulerAngles = eulerAnglesF,
                });
            }

            return new AreaMainInfo
            {
                AreaType = AreaTypes.Sphere,
                Series = seriesA,
                Center = centerA,
                Params = paramsA,
                Fullerenes = fullerenesA,
                FullereneType = FullereneTypes.Icosahedron,
            };
        }

        public static LimitedArea AdaptToLimitedArea(IReadOnlyCollection<FlatArea> flatArea)
        {
            LimitedArea area = null;
            List<Fullerene> fullerenesA = new List<Fullerene>(flatArea.Count());

            foreach (FlatArea record in flatArea)
            {
                if (area is null)
                {
                    var flatCenterA = record.CenterA;

                    // Убираем угловые скобки
                    flatCenterA = flatCenterA.Trim('<', '>').Replace(',', '.');

                    // Разделяем строку на компоненты по пробелу
                    string[] components = flatCenterA.Split(' ');

                    // Преобразуем компоненты в float и создаем Vector3
                    float xA = float.Parse(components[0].Trim(), CultureInfo.InvariantCulture);
                    float yA = float.Parse(components[1].Trim(), CultureInfo.InvariantCulture);
                    float zA = float.Parse(components[2].Trim(), CultureInfo.InvariantCulture);

                    string flatParams = record.ParamsA;

                    // Разделяем строку по "), (" и убираем скобки
                    string[] pairs = flatParams.Trim('(', ')').Split("), (", StringSplitOptions.None);

                    // Создаем массив кортежей (string, float)
                    var paramsA = pairs.Select(pair =>
                    {
                        // Разделяем пару на имя и значение
                        var parts = pair.Split(", ");
                        string name = parts[0];
                        float value = float.Parse(parts[1]);

                        return (name, value);
                    }).ToArray();

                    area = new SphereLimitedArea(
                        xA, yA, zA, paramsA[0].value,
                        record.SeriesA)
                    {
                        ProduceOctree = null,
                        ProduceFullerene = null,
                        Random = null,
                        Gamma = null
                    };
                }

                float sizeF = record.SizeF;

                var flatCenterF = record.CenterF;

                // Убираем угловые скобки
                flatCenterF = flatCenterF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                string[] componentsF = flatCenterF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float xF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float yF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float zF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                Vector3 centerF = new(xF, yF, zF);

                var flatEulerAnglesF = record.EulerAnglesF;

                // Убираем угловые скобки
                flatEulerAnglesF = flatEulerAnglesF.Trim('<', '>').Replace(',', '.');

                // Разделяем строку на компоненты по пробелу
                componentsF = flatEulerAnglesF.Split(' ');

                // Преобразуем компоненты в float и создаем Vector3
                float aF = float.Parse(componentsF[0].Trim(), CultureInfo.InvariantCulture);
                float bF = float.Parse(componentsF[1].Trim(), CultureInfo.InvariantCulture);
                float gF = float.Parse(componentsF[2].Trim(), CultureInfo.InvariantCulture);

                EulerAngles eulerAnglesF = new()
                {
                    PraecessioAngle = aF,
                    NutatioAngle = bF,
                    ProperRotationAngle = gF
                };

                fullerenesA.Add(new IcosahedronFullerene(xF, yF, zF, aF, bF, gF, sizeF));
            }

            area.Fullerenes = fullerenesA;

            return area;
        }
    }
}
