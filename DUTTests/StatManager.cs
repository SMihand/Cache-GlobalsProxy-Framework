using CacheEXTREME2.WProxyGlobal;
using EntitiesGenerationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DUTTests
{

    class DUTStatManager<StatKey, StatValue> where StatValue : class where StatKey : class
    {
        DUTManager<StatKey, StatValue> statistic;

        int statSequenceMaxCount;
        Delegate onFullSequenceHandler;
        List<StatSequence<StatKey>> sequences;

        Queue<StatKey> statQueue;

        //
        public DUTStatManager(int statSequenceMaxCount, Delegate onFullSequenceHandler)
        {
            this.statSequenceMaxCount = statSequenceMaxCount;
            this.onFullSequenceHandler = onFullSequenceHandler;
            this.sequences = new List<StatSequence<StatKey>>();
            this.statQueue = new Queue<StatKey>();
        }
        //

        public void AddStat(StatKey stat)
        {
            statQueue.Enqueue(stat);
            //
            //statQueue must be free on enqueue!!! while stat is adding to sequences
            //
            StatSequence<StatKey> newSequence = new StatSequence<StatKey>(statSequenceMaxCount, onFullSequenceHandler);
            sequences.Add(newSequence);
            for (int i = 0; i < sequences.Count; i++)
			{
                sequences[i].AddStat(stat);
                if (sequences[i].CurrentCount == statSequenceMaxCount)
                {
                    sequences.Remove(sequences[i]);
                }
            }
            //
            //??? is statQueue must be blocked after adding!!!
            //
            statQueue.Dequeue();
        }
    }

    class StatSequence<StatKey> where StatKey: class
    {
        int sequenceMaxCount;
        public int CurrentCount { get { return statKeys.Count; } }
        List<StatKey> statKeys;
        Delegate onFullCollectionHandler;
        //
        public StatSequence(int sequenceMaxCount, Delegate onFullCollectionHandler)
        {
            this.statKeys = new List<StatKey>(sequenceMaxCount);
            this.sequenceMaxCount = sequenceMaxCount;
            this.onFullCollectionHandler = onFullCollectionHandler;
        }
        //
        public void AddStat(StatKey stat)
        {
            statKeys.Add(stat);
            if (statKeys.Count == sequenceMaxCount)
            {
                onFullCollectionHandler.DynamicInvoke(new object[] { statKeys });
            }
        }
    }
}
