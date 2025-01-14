import './ControlPanel.css';
import deleteIcon from '../../assets/icons/delete.png';
import plusIcon from '../../assets/icons/plus.png';
import checkIcon from '../../assets/icons/checked.png';

function ControlPanel({ transaction, onDeleteTransaction, onAddProduct, onFinishTransaction }: ControlPanelProps) {
    const totals = transaction.lines.reduce((u, k) => u + k.amount, 0);

    return (
        <div className="controlPanel">
            <div className="transactionInfo">
                <div className="customerName">{transaction.customer.name}</div>
                <div className="totals">{totals} kr.</div>
            </div>
            <div className="commandButtons">
                <button onClick={onDeleteTransaction} className="button buttonSecondary commandButton" id="deleteTransaction">
                    <img src={deleteIcon}></img>
                </button>

                <button onClick={onAddProduct} className="button buttonPrimary commandButton" id="addItem">
                    <img src={plusIcon}></img>
                </button>

                <button onClick={onFinishTransaction} className="button buttonPrimary commandButton" id="finishTransaction">
                    <img src={checkIcon}></img>
                </button>
            </div>
        </div>
    );
}

export default ControlPanel;