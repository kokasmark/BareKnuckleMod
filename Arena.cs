using System.Numerics;
using DeadworksManaged.Api.Utils;

namespace BareKnucleMod;

public class ArenaCamera
{
    public float CameraFov = 100f;
    public float AspectRatio = 16f / 9f;
    public float CameraMargin = 1.25f;
    public float CameraHeight = 100f;
}

public sealed class Arena
{
    public string Name { get; }
    public string Key => $"map_{Name.Replace(" ", "_").ToLowerInvariant()}";
    public Zone Zone { get; }
    public float Orientation { get; }
    public ArenaCamera CameraSettings { get; }
    public Vector3 Forward { get; }
    public Vector3 Right { get; }
    public float Width { get; }
    public float Depth { get; }

    public Vector3 SpawnPoint => new(Zone.Center.X, Zone.Center.Y, Zone.Mins.Z + Globals.SpawnLift);
    public Vector3 CameraAngles => new(0f, Orientation, 0f);

    public Arena(string name, Zone zone, float orientation, ArenaCamera cameraSettings)
    {
        Name = name;
        Zone = zone;
        Orientation = orientation;
        CameraSettings = cameraSettings;

        var radians = orientation * MathF.PI / 180f;
        Forward = new Vector3(MathF.Cos(radians), MathF.Sin(radians), 0f);
        Right = new Vector3(Forward.Y, -Forward.X, 0f);
        Width = MathF.Abs(Right.X) * zone.Size.X + MathF.Abs(Right.Y) * zone.Size.Y;
        Depth = MathF.Abs(Forward.X) * zone.Size.X + MathF.Abs(Forward.Y) * zone.Size.Y;

    }

    public bool TryReturn(Vector3 position, out Vector3 corrected)
    {
        var offset = position - Zone.Center;
        var along = Vector3.Dot(offset, Right);
        var across = Vector3.Dot(offset, Forward);
        var halfWidth = Width * 0.5f;
        var halfDepth = Depth * 0.5f;

        corrected = position;

        if (MathF.Abs(along) <= halfWidth + Globals.BoundsInset && MathF.Abs(across) <= halfDepth + Globals.BoundsInset)
            return false;

        var limit = MathF.Max(halfWidth - Globals.BoundsInset, 0f);
        var lane = Zone.Center + Right * Math.Clamp(along, -limit, limit);

        corrected = new Vector3(lane.X, lane.Y, position.Z);
        return true;
    }

    public Vector3 CameraPosition
    {
        get
        {
            var halfWidth = MathF.Tan(CameraSettings.CameraFov * 0.5f * MathF.PI / 180f);

            var distance = (Width * 0.5f / halfWidth) * CameraSettings.CameraMargin;

            var position = Zone.Center - Forward * distance;

            return new Vector3(
                position.X,
                position.Y,
                Zone.Center.Z + CameraSettings.CameraHeight
            );
        }
    }
}