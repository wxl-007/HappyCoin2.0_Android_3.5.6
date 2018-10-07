using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameCenterScore
{
	public string category;
	public string formattedValue;
	public int value;
	public DateTime date;
	public string playerId;
	public int rank;
	public bool isFriend;
	public string alias;
	
	
	public static List<GameCenterScore> fromJSON( string json )
	{
		var scoreList = new List<GameCenterScore>();
		
		// decode the json
		var list = json.arrayListFromJson();
		
		// create DTO's from the Hashtables
		foreach( Hashtable ht in list )
			scoreList.Add( new GameCenterScore( ht ) );
		
		return scoreList;
	}
	
	
	public GameCenterScore( Hashtable ht )
	{
		if( ht.Contains( "category" ) )
			category = ht["category"] as string;
		
		if( ht.Contains( "formattedValue" ) )
			formattedValue = ht["formattedValue"] as string;
		
		if( ht.Contains( "value" ) )
			value = int.Parse( ht["value"].ToString() );
		
		if( ht.Contains( "playerId" ) )
			playerId = ht["playerId"] as string;
		
		if( ht.Contains( "rank" ) )
			rank = int.Parse( ht["rank"].ToString() );
		
		if( ht.Contains( "isFriend" ) )
			isFriend = (bool)ht["isFriend"];
		
		if( ht.Contains( "alias" ) )
			alias = ht["alias"] as string;
		
		// grab and convert the date
		if( ht.Contains( "date" ) )
		{
			double timeSinceEpoch = double.Parse( ht["date"].ToString() );
			DateTime intermediate = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
			date = intermediate.AddSeconds( timeSinceEpoch );
		}
	}
	
	
	public override string ToString()
	{
		 return string.Format( "<Score> category: {0}, formattedValue: {1}, date: {2}, rank: {3}, alias: {4}",
			category, formattedValue, date, rank, alias );
	}

}