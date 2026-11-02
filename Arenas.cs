using System.Numerics;
using DeadworksManaged.Api.Utils;

namespace BareKnucleMod;

public static class Arenas
{
    public static Arena Choose()
    {
        return All[Random.Shared.Next(All.Length)];
    }

    public static readonly Arena Arch = new(
        "Archway",
        Zone.FromCenter(
            new Vector3(3930f, 25f, 910f),
            new Vector3(200f, 800f, 200f)
        ),
        0,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 0.75f,
            CameraHeight = 100f,
        }
    );

    public static readonly Arena Broadway = new(
        "Broadway",
        Zone.FromCenter(
            new Vector3(0f, 70f, 640f),
            new Vector3(1800f, 200f, 200f)
        ),
        90,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 0.5f,
            CameraHeight = 100f,
        }
    );

    public static readonly Arena TheOracle = new(
        "The Oracle",
        Zone.FromCenter(
            new Vector3(1323f, 698f, 3305f),
            new Vector3(813f, 30f, 200f)
        ),
        90,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.5f,
            CameraHeight = 100f,
        }
    );

    public static readonly Arena Dome = new(
        "Dome",
        Zone.FromCenter(
            new Vector3(1157f, 5955f, 3771f),
            new Vector3(1768f, 20f, 253f)
        ),
        90,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 300f,
        }
    );

    public static readonly Arena Bridge = new(
        "Bridge",
        Zone.FromCenter(
            new Vector3(0f, -1326f, 3350f),
            new Vector3(1073f, 113f, 200f)
        ),
        90,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 100f,
        }
    );

    public static readonly Arena Theater = new(
        "Theater",
        Zone.FromCenter(
            new Vector3(4176f, 1723f, 400f),
            new Vector3(757f, 20f, 200f)
        ),
        270,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 50f,
        }
    );

    public static readonly Arena Theater2 = new(
            "Theater 2",
            Zone.FromCenter(
                new Vector3(4160f, 2343f, 858f),
                new Vector3(980f, 100f, 400f)
            ),
            90,
            new ArenaCamera
            {
                CameraFov = 100f,
                AspectRatio = 16f / 9f,
                CameraMargin = 0.75f,
                CameraHeight = 0f,
            }
        );

    public static readonly Arena HiddenKing = new(
        "Hidden King",
        Zone.FromCenter(
            new Vector3(-1354f, -7500f, 800f),
            new Vector3(964f, 65f, 200f)
        ),
        270,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 50f,
        }
    );

    public static readonly Arena ArchMother = new(
        "Arch Mother",
        Zone.FromCenter(
            new Vector3(1238f, 7525f, 744f),
            new Vector3(812f, 136f, 200f)
        ),
        90,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 50f,
        }
    );

    public static readonly Arena BellTower = new(
        "Bell Tower",
        Zone.FromCenter(
            new Vector3(3125f, -1844f, 2655f),
            new Vector3(137f, 1172f, 200f)
        ),
        0,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 6f,
            CameraHeight = 50f,
        }
    );

    public static readonly Arena ChinaTown = new(
        "China Town",
        Zone.FromCenter(
            new Vector3(4132f, -3801f, 475f),
            new Vector3(798f, 33f, 200f)
        ),
        270,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.25f,
            CameraHeight = 50f,
        }
    );

    public static readonly Arena GreenWalker = new(
        "Green Walker",
        Zone.FromCenter(
            new Vector3(5623f, -5041f, 476f),
            new Vector3(1350f, 100f, 400f)
        ),
        270,
        new ArenaCamera
        {
            CameraFov = 100f,
            AspectRatio = 16f / 9f,
            CameraMargin = 1.5f,
            CameraHeight = 100f,
        }
    );

    public static readonly Arena[] All = [Arch, Broadway, TheOracle, Dome, Bridge, Theater, Theater2, HiddenKing, ArchMother, BellTower, ChinaTown, GreenWalker];
}