using Nintendo.Byml;
using System.Numerics;
using Yaz0Library;

namespace WarpPointEditor.Core;

public record StartPos(string Map, string PlayerState, string PosName, Vector3 Translate, Vector3 Rotate);
public record LocationMarker(string? Icon, string? MessageID, int Priority, string SaveFlag, Vector3 Translate, string? WarpDestMapName, string? WarpDestPosName);
public record MapStaticEntry(string MarkerMap, LocationMarker Marker, string? PosMap, StartPos? Pos);

public class MapStatic : List<MapStaticEntry>
{
    private readonly BymlFile _aocStatic;
    private readonly BymlFile _mainStatic;

    private static List<BymlNode> LocationMarkerArray(BymlFile src) => src.RootNode.Hash.TryGetValue("LocationMarker", out BymlNode? node) ? node.Array : new();
    private List<BymlNode> LocationMarkerArray(string map)
    {
        BymlFile src = (map == "AocField" ? _aocStatic : _mainStatic);
        return LocationMarkerArray(src);
    }

    private static List<BymlNode> StartPosArray(BymlFile src) => src.RootNode.Hash.TryGetValue("StartPos", out BymlNode? node) ? node.Array : new();
    private List<BymlNode> StartPosArray(string map)
    {
        BymlFile src = map == "AocField" ? _aocStatic : _mainStatic;
        return StartPosArray(src);
    }

    public MapStatic(string main, string aoc)
    {
        // TODO: (MapStatic) Two functions for the same job on different variables is used far to often

        _aocStatic = BymlFile.FromBinary(Yaz0.Decompress(aoc).ToArray());
        _mainStatic = BymlFile.FromBinary(Yaz0.Decompress(main).ToArray());

        foreach (var entry in LocationMarkerArray(_aocStatic)) {
            Add(BuildMapEntry("AocField", entry.Hash));
        }

        foreach (var entry in LocationMarkerArray(_mainStatic)) {
            Add(BuildMapEntry("MainField", entry.Hash));
        }

        StartPosArray(_aocStatic).Clear();
        LocationMarkerArray(_aocStatic).Clear();

        StartPosArray(_mainStatic).Clear();
        LocationMarkerArray(_mainStatic).Clear();
    }

    public void Write(string main, string aoc)
    {
        foreach (var entry in this) {
            LocationMarkerArray(entry.MarkerMap).Add(BuildNode(entry.Marker));

            if (entry.PosMap != null) {
                StartPosArray(entry.PosMap).Add(BuildNode(entry.Pos));
            }
        }

        byte[] aocData = _aocStatic.ToBinary();
        File.WriteAllBytes(aoc, Yaz0.Compress(aocData, out Yaz0SafeHandle _).ToArray());

        byte[] mainData = _mainStatic.ToBinary();
        File.WriteAllBytes(main, Yaz0.Compress(mainData, out Yaz0SafeHandle _).ToArray());
    }

    private MapStaticEntry BuildMapEntry(string srcMapName, SortedDictionary<string, BymlNode> entry)
    {
        // TODO: (MapStatic) Clean this mess up

        LocationMarker marker = new(
            entry.TryGetValue("Icon", out BymlNode? iconNode) ? iconNode?.String : null,
            entry.TryGetValue("MessageID", out BymlNode? msgNode) ? msgNode?.String : null,
            entry["Priority"].Int,
            entry["SaveFlag"].String,
            HashToVector(entry["Translate"].Hash),
            entry.TryGetValue("WarpDestMapName", out BymlNode? wdmnNode) ? wdmnNode?.String : null,
            entry.TryGetValue("WarpDestPosName", out BymlNode? wdpnNode) ? wdpnNode?.String : null
        );

        StartPos? pos = null;
        string? map = null;

        if (entry.TryGetValue("WarpDestMapName", out BymlNode? warpDestMapNameNode) && entry.TryGetValue("WarpDestPosName", out BymlNode? warpDestPosName)) {
            var warpDestMapName = warpDestMapNameNode!.String.Split('/');
            (map, string field) = (warpDestMapName[0], warpDestMapName[1]);

            SortedDictionary<string, BymlNode>? startPos = StartPosArray(map).Where(
                x => x.Hash["Map"].String == field && x.Hash.TryGetValue("PosName", out BymlNode? posNameNode) && posNameNode!.String == warpDestPosName!.String
            ).FirstOrDefault()?.Hash;
            
            pos = startPos != null ? new(
                field, startPos["PlayerState"].String, startPos["PosName"].String, HashToVector(startPos["Translate"].Hash), HashToVector(startPos["Rotate"].Hash)) : null;
        }


        return new(srcMapName, marker, map, pos);
    }

    private static BymlNode BuildNode<T>(T? obj)
    {
        Dictionary<string, BymlNode> hash = new();

        foreach (var prop in typeof(T).GetProperties()) {
            if (obj != null) {
                object? value = prop.GetValue(obj);
                if (value is Vector3 vector) {
                    hash.Add(prop.Name, VectorToHash(vector));
                }
                else if (value is string str) {
                    hash.Add(prop.Name, new(str));
                }
                else if (value is int integer) {
                    hash.Add(prop.Name, new(integer));
                }
            }
        }

        return new(hash);
    }

    private static BymlNode VectorToHash(Vector3 vector)
    {
        return new(new Dictionary<string, BymlNode> {
            { "X", new(vector.X) },
            { "Y", new(vector.Y) },
            { "Z", new(vector.Z) },
        });
    }

    private static Vector3 HashToVector(SortedDictionary<string, BymlNode> hash)
    {
        return new() {
            X = hash["X"].Float,
            Y = hash["Y"].Float,
            Z = hash["Z"].Float
        };
    }
}
