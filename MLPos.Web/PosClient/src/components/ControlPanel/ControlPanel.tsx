import "./ControlPanel.css";
import deleteIcon from "../../assets/icons/delete.png";
import plusIcon from "../../assets/icons/plus.png";
import checkIcon from "../../assets/icons/checked.png";

function ControlPanel({
  transaction,
  onDeleteTransaction,
  onAddProduct,
  onFinishTransaction,
}: ControlPanelProps) {
  const totals = transaction.lines.reduce((u, k) => u + k.amount, 0);

  return (
    <div className="controlPanel">
      <div className="transactionInfo">
        <div className="customerName">{transaction.customer.name}</div>
        <div className="totals">{totals} kr.</div>
      </div>
      <div className="commandButtons">
        <button
          disabled={transaction.id == -1}
          onClick={onDeleteTransaction}
          className={
            "button buttonSecondary commandButton " +
            (transaction.id == -1 && "disabled")
          }
          id="deleteTransaction"
        >
          <img src={deleteIcon}></img>
        </button>

        <button
          disabled={transaction.id == -1}
          onClick={onAddProduct}
          className={
            "button buttonPrimary commandButton " +
            (transaction.id == -1 && "disabled")
          }
          id="addItem"
        >
          <img src={plusIcon}></img>
        </button>

        <button
          disabled={transaction.id == -1}
          onClick={onFinishTransaction}
          className={
            "button buttonPrimary commandButton " +
            (transaction.id == -1 && "disabled")
          }
          id="finishTransaction"
        >
          <img src={checkIcon}></img>
        </button>
      </div>
    </div>
  );
}

export default ControlPanel;
