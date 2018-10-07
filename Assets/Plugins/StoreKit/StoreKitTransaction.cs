using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_IPHONE
public class StoreKitTransaction
{
    public string productIdentifier;
    public string base64EncodedTransactionReceipt;
    public int quantity;
	
	
	
	public static List<StoreKitTransaction> transactionsFromJson( string json )
	{
		var transactionList = new List<StoreKitTransaction>();
		
		ArrayList products = json.arrayListFromJson();
		foreach( Hashtable ht in products )
			transactionList.Add( transactionFromHashtable( ht ) );
		
		return transactionList;
	}
	

    public static StoreKitTransaction transactionFromJson( string json )
    {
		return transactionFromHashtable( json.hashtableFromJson() );
    }
	
	
    public static StoreKitTransaction transactionFromHashtable( Hashtable ht )
    {
        var transaction = new StoreKitTransaction();
  		
		if( ht.ContainsKey( "productIdentifier" ) )
        	transaction.productIdentifier = ht["productIdentifier"].ToString();
		
		if( ht.ContainsKey( "base64EncodedReceipt" ) )
        	transaction.base64EncodedTransactionReceipt = ht["base64EncodedReceipt"].ToString();
		
		if( ht.ContainsKey( "quantity" ) )
        	transaction.quantity = int.Parse( ht["quantity"].ToString() );

        return transaction;
    }
	
	
	public override string ToString()
	{
		return string.Format( "<StoreKitTransaction>\nID: {0}\nReceipt: {1}\nQuantity: {2}", productIdentifier, base64EncodedTransactionReceipt, quantity );
	}

}
#endif
