﻿namespace Coosu.Beatmap.Configurable;

public interface ISection : ISerializeWritable
{
    void Match(string line);

}