import "./TransactionSummary.css";
import { useContext } from "react";
import { TransactionContext } from "../../context";

function TransactionSummary({
  transaction,
}: {
  transaction: TransactionSummary;
}) {
  const { activeTransactionId, setActiveTransactionId } =
    useContext(TransactionContext);

  const transactionClicked = () => {
    setActiveTransactionId(transaction.id);
  };

  return (
    <div
      className={
        "container transactionSummaryContainer " +
        (activeTransactionId === transaction.id ? "activeTransaction" : "")
      }
      onClick={transactionClicked}
    >
      <div className="transactionImageContainer">
        <div className="transactionImagePlaceholder">
          {transaction.customerName[0]}
        </div>
        <img className="transactionImage" src={transaction.customerImage}></img>
      </div>
      <div className="transactionSummaryDetails">
        <div className="transactionSummaryName">{transaction.customerName}</div>
      </div>
    </div>
  );
}

export default TransactionSummary;
