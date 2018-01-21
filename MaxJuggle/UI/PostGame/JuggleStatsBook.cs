// Programmer: Carl Childers
// Date: 9/8/2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggleStatsBook : RecordBook {

	//public JuggleObjectType DefaultJuggleType;
	public string NoJuggleMessage;

	int numRecords;


	void Start()
	{
		JuggleStatKeeper statKeeper = JuggleStatKeeper.GetStatKeeper();
		Dictionary<JuggleObjectType, int> aggregateStats = GetAggregatedJuggleStats(statKeeper.FullyJuggledObjects);
		bool addedRecords = false;
		if (aggregateStats != null)
		{
			numRecords = aggregateStats.Keys.Count;

			if (aggregateStats.Count > 0)
			{
				addedRecords = true;
			}
			foreach (JuggleObjectType t in aggregateStats.Keys)
			{
				int juggleCount = aggregateStats[t];
				string recordText = juggleCount.ToString() + " " + (juggleCount == 1 ? t.SingularName : t.PluralName);
				AddRecord(recordText);
			}
		}

		if (!addedRecords)
		{
			AddRecord(NoJuggleMessage);
		}
	}

	// Center records if there's only one column
	protected override Vector2 GetRecordPosition(int column, int row)
	{
		Vector2 recordPos = base.GetRecordPosition(column, row);
		if (numRecords < 4)
		{
			recordPos.x = 0;
		}
		return recordPos;
	}

	Dictionary<JuggleObjectType, int> GetAggregatedJuggleStats(Dictionary<JuggleObjectType, int> inStats)
	{
		if (inStats == null)
		{
			return null;
		}

		Dictionary<JuggleObjectType, int> outputStats = new Dictionary<JuggleObjectType, int>(inStats.Keys.Count);
		foreach (JuggleObjectType key in inStats.Keys)
		{
			bool foundSimilarType = false;
			foreach (JuggleObjectType outKey in outputStats.Keys)
			{
				// Combine stats that share the same singular name
				if (outKey.SingularName == key.SingularName)
				{
					outputStats[outKey] += inStats[key];
					foundSimilarType = true;
					break;
				}
			}

			// If a similar type was not found, then add a new key value pair to the output stats
			if (!foundSimilarType)
			{
				outputStats.Add(key, inStats[key]);
			}
		}

		return outputStats;
	}
}
