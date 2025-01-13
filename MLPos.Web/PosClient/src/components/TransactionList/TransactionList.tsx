import './TransactionList.css';
import TransactionSummary from '../TransactionSummary/TransactionSummary';

function TransactionList({ transactions }: TransactionListProps) {

    return <div className="container transactionSelectionContainer">
        {transactions.map(transaction =>
            <TransactionSummary transaction={transaction} key={transaction.id} />
        )}
    </div>
}

export default TransactionList;