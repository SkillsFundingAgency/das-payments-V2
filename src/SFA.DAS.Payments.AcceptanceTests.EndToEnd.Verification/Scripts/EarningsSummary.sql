

select
	isnull(sum(case when Amount > 0 then Amount end), 0) [EarningsYTD]--,
	/*isnull(sum(case when ContractType = 1 and Amount > 0 then Amount end), 0) [EarningsACT1],
	isnull(sum(case when ContractType = 2 and Amount > 0 then Amount end), 0) [EarningsACT2],
	isnull(sum(case when Amount < 0 then Amount end), 0) [NegativeEarningsYTD],
	isnull(sum(case when ContractType = 1 and Amount < 0 then Amount end), 0) [NegativeEarningsACT1],
	isnull(sum(case when ContractType = 2 and Amount < 0 then Amount end), 0) [NegativeEarningsACT2]*/
from (
		select n as TransactionType from (values (1),(2),(3)) v(n)
		--select n as TransactionType from (values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14),(15),(16)) v(n)
	) as TransactionTypes		
	left join [Earnings] e on e.TransactionType = TransactionTypes.TransactionType
