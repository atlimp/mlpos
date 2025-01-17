import "./TransactionList.css";
import TransactionSummary from "../TransactionSummary/TransactionSummary";
import plusIcon from "../../assets/icons/plus.png";
import { useState, useContext } from "react";
import { PosClientIdContext } from "../../context";
import Api from "../../api/api";
import CustomerSelect from "../CustomerSelect/CustomerSelect";

function TransactionList({
  transactions,
  refreshTransactions,
}: TransactionListProps) {
  const { posClientId } = useContext(PosClientIdContext);
  const [customerSelect, setCustomerSelect] = useState<boolean>(false);

  const onCustomerSelected = async (customerId: number) => {
    setCustomerSelect(false);
    if (customerId === -1) return;

    const api = new Api({ posClientId });
    await api.createTransaction(customerId);
    refreshTransactions();
  };

  return (
    <div className="container transactionSelectionContainer">
      {customerSelect && (
        <CustomerSelect onSelectCustomer={onCustomerSelected}></CustomerSelect>
      )}
      <div
        className="transactionSummaryContainer"
        onClick={() => {
          setCustomerSelect(true);
        }}
      >
        <div className="transactionImageContainer">
          <img className="newTransactionImage" src={plusIcon}></img>
        </div>

        <div className="transactionSummaryDetails">
          <div className="transactionName">Add transaction</div>
          <div className="transactionTotal"></div>
        </div>
      </div>
      {transactions.map((transaction) => (
        <TransactionSummary transaction={transaction} key={transaction.id} />
      ))}
    </div>
  );
}

export default TransactionList;
