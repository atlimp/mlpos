import './Pos.css';
import LineDisplay from "../LineDisplay/LineDisplay";
import ControlPanel from '../ControlPanel/ControlPanel';
import ProductSelect from '../ProductSelect/ProductSelect';
import { useContext, useEffect, useState } from 'react';
import { PosClientIdContext, TransactionContext } from '../../context';
import Api from '../../api/api';
function Pos({ refreshTransactions }) {
    const { activeTransactionId } = useContext(TransactionContext);
    const { posClientId } = useContext(PosClientIdContext);
    const [activeTransaction, setActiveTransaction] = useState<Transaction>();
    const [productSelect, setProductSelect] = useState<boolean>(false);

    const getActiveTransaction = async (posClientId: number, activeTransactionId: number) => {
        const api = new Api({ posClientId });
        const transaction = await api.getTransaction(activeTransactionId);
        if (transaction)
            setActiveTransaction(transaction);
    }

    const onDeleteLine = async (lineId) => {
        const api = new Api({ posClientId });
        const transaction = await api.deleteTransactionLine(activeTransactionId, lineId);
        if (transaction)
            setActiveTransaction(transaction);
    }

    const onDeleteTransaction = async () => {
        const api = new Api({ posClientId });
        if (await api.deleteTransaction(activeTransactionId)) {
            refreshTransactions();
        }
    }

    const onProductSelection = () => {
        setProductSelect(true);
    }

    const onProductSelected = async (productId: number) => {
        setProductSelect(false);
        console.log(productId)
        if (productId === -1)
            return;

        const api = new Api({ posClientId });
        const transaction = await api.addTransactionLine(activeTransactionId, productId);
        if (transaction)
            setActiveTransaction(transaction);
    }

    useEffect(() => {
        getActiveTransaction(posClientId, activeTransactionId);
    }, [posClientId, activeTransactionId]);

    if (!activeTransaction) {
        return <div>Loading...</div>
    }

    return <div className="pos">
        {productSelect && <ProductSelect onSelectProduct={onProductSelected}></ProductSelect>}
        <LineDisplay lines={activeTransaction?.lines ?? []} onDeleteLine={onDeleteLine} />
        <ControlPanel
            transaction={activeTransaction}
            onDeleteTransaction={onDeleteTransaction}
            onAddItem={onProductSelection} />        
    </div>
}

export default Pos;