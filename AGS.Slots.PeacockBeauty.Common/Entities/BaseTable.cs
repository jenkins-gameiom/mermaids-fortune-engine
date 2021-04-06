using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Entities
{
    public class BaseTable
    {
        public List<List<int>> outcome { get; set; }
        public List<int[]> weights { get; set; }


        //public List<List<int>> bucket_sort_table_symbols { get; set; }
        //
        //[System.Runtime.Serialization.OnDeserialized]
        //public void CalcBucketSort(StreamingContext context)
        //{
        //    bucket_sort_table_symbols = new List<List<int>>();
        //    for (int reelidx = 0; reelidx < lookup_table_symbols.Count(); reelidx++)
        //    {
        //        bucket_sort_table_symbols.Add(new List<int>());
        //        for (int symbolidx = 0; symbolidx < lookup_table_symbols[reelidx].Count(); symbolidx++)
        //        {
        //            for (int weightreoccur = 0; weightreoccur < lookup_table_weights[reelidx][symbolidx]; weightreoccur++)
        //            {
        //                bucket_sort_table_symbols[reelidx].Add(symbolidx);
        //            }
        //        }
        //    }
        //}
    }
}
