﻿using System.Collections.Generic;
using Intervals;
using VariantAnnotation.Interface.AnnotatedPositions;
using VariantAnnotation.Interface.Providers;
using VariantAnnotation.IO.Caches;
using VariantAnnotation.Providers;

namespace VariantAnnotation.Caches
{
    public sealed class TranscriptCacheData
    {
        public readonly CacheHeader Header;
        
        public readonly IGene[] Genes;
        public readonly ITranscriptRegion[] TranscriptRegions;
        public readonly IInterval[] Mirnas;
        public readonly string[] PeptideSeqs;
        public readonly IntervalArray<ITranscript>[] TranscriptIntervalArrays;
        public readonly IntervalArray<IRegulatoryRegion>[] RegulatoryRegionIntervalArrays;

        public TranscriptCacheData(CacheHeader header, IGene[] genes, ITranscriptRegion[] transcriptRegions,
            IInterval[] mirnas, string[] peptideSeqs, IntervalArray<ITranscript>[] transcriptIntervalArrays,
            IntervalArray<IRegulatoryRegion>[] regulatoryRegionIntervalArrays)
        {
            Header                         = header;
            Genes                          = genes;
            TranscriptRegions              = transcriptRegions;
            Mirnas                         = mirnas;
            PeptideSeqs                    = peptideSeqs;
            TranscriptIntervalArrays       = transcriptIntervalArrays;
            RegulatoryRegionIntervalArrays = regulatoryRegionIntervalArrays;
        }

        public TranscriptCache GetCache()
        {
            var dataSourceVersions = GetDataSourceVersions(Header);
            return new TranscriptCache(dataSourceVersions, Header.Assembly, TranscriptIntervalArrays, RegulatoryRegionIntervalArrays);
        }

        private static IEnumerable<IDataSourceVersion> GetDataSourceVersions(CacheHeader header)
        {
            var dataSourceVersions = new List<IDataSourceVersion>();
            if (header == null) return dataSourceVersions;

            ushort vepVersion = header.Custom.VepVersion;

            var dataSourceVersion = new DataSourceVersion("VEP", vepVersion.ToString(), header.CreationTimeTicks, header.Source.ToString());
            dataSourceVersions.Add(dataSourceVersion);
            return dataSourceVersions;
        }
    }
}
