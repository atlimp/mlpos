import './TransactionSummary.css';
import { useContext } from 'react';
import { TransactionContext } from '../../context';

function TransactionSummary({ transaction }: { transaction: TransactionSummary }) {

    const { activeTransactionId, setActiveTransactionId } = useContext(TransactionContext);

    const transactionClicked = () => {
        setActiveTransactionId(transaction.id);
    }

    return <div className={"container transactionSummaryContainer " + (activeTransactionId === transaction.id ? "activeTransaction" : "")} onClick={transactionClicked}>
                <div className="transactionImageContainer">
                    <img className="transactionImage" src={transaction.customerImage}></img>
                </div>
           </div>
}

export default TransactionSummary;