﻿using System;
using System.Collections.Generic;
using System.IO;
using CacheUtils.Genbank;
using VariantAnnotation.Interface.Intervals;

namespace CacheUtils.IntermediateIO
{
    internal sealed class GenbankReader : IDisposable
    {
        private readonly StreamReader _reader;

        internal GenbankReader(Stream stream)
        {
            _reader = new StreamReader(stream);
            IntermediateIoCommon.ReadHeader(_reader, IntermediateIoCommon.FileType.Genbank);
        }

        public Dictionary<string, GenbankEntry> GetIdToGenbank()
        {
            var genbankDict = new Dictionary<string, GenbankEntry>();

            while (true)
            {
                var entry = GetNextEntry();
                if (entry == null) break;
                genbankDict[entry.TranscriptId] = entry;
            }

            return genbankDict;
        }

        private GenbankEntry GetNextEntry()
        {
            var line = _reader.ReadLine();
            if (line == null) return null;

            var info  = ReadTranscriptInfo(line);
            var exons = ReadExons(info.NumExons);

            return new GenbankEntry(info.TranscriptId, info.TranscriptVersion, info.ProteinId, info.ProteinVersion,
                info.GeneId, info.GeneSymbol, info.CodingRegion, exons);
        }

        private IInterval[] ReadExons(int numExons)
        {
            if (numExons == 0) return null;

            var line = _reader.ReadLine();
            if (line == null) throw new InvalidOperationException("Unexpected null line when parsing exons");

            var cols = line.Split('\t');
            if (cols[0] != "Exons") throw new InvalidDataException($"Expected the first keyword to be Exons, but found something different: {line}");

            var exons = new IInterval[numExons];
            int colIndex = 1;

            for (int i = 0; i < numExons; i++)
            {
                int start = int.Parse(cols[colIndex++]);
                int end   = int.Parse(cols[colIndex++]);
                exons[i]  = new Interval(start, end);
            }

            return exons;
        }

        private static (string TranscriptId, byte TranscriptVersion, string ProteinId, byte ProteinVersion, string
            GeneId, string GeneSymbol, IInterval CodingRegion, int NumExons) ReadTranscriptInfo(string line)
        {
            var cols = line.Split('\t');
            if (cols.Length != 9) throw new InvalidDataException($"Expected 9 columns, but found {cols.Length} columns instead.");

            var transcriptId      = cols[0];
            var transcriptVersion = byte.Parse(cols[1]);
            var proteinId         = cols[2];
            var proteinVersion    = byte.Parse(cols[3]);
            var geneId            = cols[4];
            var geneSymbol        = cols[5];
            var start             = int.Parse(cols[6]);
            var end               = int.Parse(cols[7]);
            var numExons          = int.Parse(cols[8]);

            var codingRegion = new Interval(start, end);
            return (transcriptId, transcriptVersion, proteinId, proteinVersion, geneId, geneSymbol, codingRegion,
                numExons);
        }

        public void Dispose() => _reader.Dispose();
    }
}