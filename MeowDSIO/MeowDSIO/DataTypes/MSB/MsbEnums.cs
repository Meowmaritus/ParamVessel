using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.DataTypes.MSB
{
    //public enum MsbValueType
    //{
    //    None = 0,
    //    Byte,
    //    Short,
    //    Int,
    //    String,
    //    Float,
    //}

    //public enum MsbParamType
    //{
    //    MODEL_PARAM_ST,
    //    EVENT_PARAM_ST,
    //    POINT_PARAM_ST,
    //    PARTS_PARAM_ST,
    //}

    public enum EventParamSubtype : int
    {
        Lights = 0,
        Sounds = 1,
        SFX = 2,
        WindSFX = 3,
        Treasures = 4,
        Generators = 5,
        BloodMsg = 6,
        ObjActs = 7,
        SpawnPoints = 8,
        MapOffset = 9,
        Navimesh = 10,
        Environment = 11,
        BlackEyeOrbInvasions = 12,
    }

    public enum PointParamSubtype : int
    {
        Points = 0,
        Spheres = 2,
        Cylinders = 3,
        Boxes = 5,
    }

    public enum PartsParamSubtype : int
    {
        MapPieces = 0,
        Objects = 1,
        NPCs = 2,
        Players = 4,
        Collisions = 5,
        Navimeshes = 8,
        UnusedObjects = 9,
        UnusedNPCs = 10,
        UnusedCollisions = 11,
    }

    public enum ModelParamSubtype : int
    {
        MapPiece = 0,
        Object = 1,
        Character = 2,
        Player = 4,
        Collision = 5,
        Navimesh = 6
    }

    //Events

    public enum MsbSoundType : int
    {
        //a: 環境音
        Environment = 0,
        //c: キャラモーション
        Character = 1,
        //f: メニューSE
        Menu = 2,
        //o: オブジェクト
        Object = 3,
        //p: ポリ劇専用SE
        PolySinglePlay = 4,
        //s: SFX
        SFX = 5,
        //m: BGM
        BGM = 6,
        //v: 音声
        Voice = 7,
        //x: 床材質依存
        FloorMatDetermined = 8,
        //b: 鎧材質依存
        ArmorMatDetermined = 9,
        //g: ゴースト
        Ghost = 10, 

    }
}
