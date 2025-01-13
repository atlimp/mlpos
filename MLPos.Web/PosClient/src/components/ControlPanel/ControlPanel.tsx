import './ControlPanel.css';

function ControlPanel({ transaction, onDeleteTransaction, onAddItem, onFinishTransaction }: ControlPanelProps) {
    const totals = transaction.lines.reduce((u, k) => u + k.amount, 0);

    return (
        <div className="controlPanel">
            <div className="transactionInfo">
                <div className="customerName">{transaction.customer.name}</div>
                <div className="totals">{totals} kr.</div>
            </div>
            <button onClick={onDeleteTransaction} className="button buttonSecondary" id="deleteTransaction">
                Delete transaction
            </button>

            <button onClick={onAddItem} className="button buttonPrimary" id="addItem">
                Add item
            </button>

            <button onClick={onFinishTransaction} className="button buttonPrimary" id="finishTransaction">
                Finish transaction
            </button>
        </div>
    );
}

export default ControlPanel;