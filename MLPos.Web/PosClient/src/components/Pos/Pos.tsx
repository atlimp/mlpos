﻿import "./Pos.css";
import LineDisplay from "../LineDisplay/LineDisplay";
import ControlPanel from "../ControlPanel/ControlPanel";
import ProductSelect from "../ProductSelect/ProductSelect";
import { useContext, useEffect, useState } from "react";
import {
  PosClientIdContext,
  TransactionContext,
  LocalizedStringsContext,
} from "../../context";
import Api from "../../api/api";
import PaymentMethodSelect from "../PaymentMethodSelect/PaymentMethodSelect";
import ConfirmDialog from "../ConfirmDialog/ConfirmDialog";
function Pos({ refreshTransactions }: PosProps) {
  const { activeTransactionId } = useContext(TransactionContext);
  const { posClientId } = useContext(PosClientIdContext);
  const { localizedStrings } = useContext(LocalizedStringsContext);
  const [activeTransaction, setActiveTransaction] = useState<Transaction>();
  const [productSelect, setProductSelect] = useState<boolean>(false);
  const [confirmDelete, setConfirmDelete] = useState<boolean>(false);
  const [paymentMethodSelect, setPaymentMethodSelect] =
    useState<boolean>(false);

  const getActiveTransaction = async (
    posClientId: number,
    activeTransactionId: number,
  ) => {
    if (activeTransactionId === -1) {
      const emptyTransaction: Transaction = {
        id: -1,
        customer: {
          name: "",
          email: "",
          image: "",
          id: 0,
          dateInserted: new Date(),
          dateUpdated: new Date(),
        },
        lines: [],
      };

      setActiveTransaction(emptyTransaction);

      return;
    }

    const api = new Api({ posClientId });
    const transaction = await api.getTransaction(activeTransactionId);
    if (transaction) setActiveTransaction(transaction);
  };

  const onDeleteLine = async (lineId: number) => {
    const api = new Api({ posClientId });
    const transaction = await api.deleteTransactionLine(
      activeTransactionId,
      lineId,
    );
    if (transaction) setActiveTransaction(transaction);
  };

  const onDeleteTransaction = async () => {
    setConfirmDelete(true);
  };

  const deleteTransaction = async () => {
    const api = new Api({ posClientId });
    if (await api.deleteTransaction(activeTransactionId)) {
      refreshTransactions();
    }

    setConfirmDelete(false);
  };

  const onPostTransaction = async () => {
    setPaymentMethodSelect(true);
  };

  const onPaymentMethodSelected = async (paymentMethodId: number) => {
    setPaymentMethodSelect(false);
    if (paymentMethodId === -1) return;

    const api = new Api({ posClientId });
    await api.postTransaction(activeTransactionId, paymentMethodId);
    refreshTransactions();
  };

  const onProductSelection = () => {
    setProductSelect(true);
  };

  const onProductSelected = async (productId: number) => {
    setProductSelect(false);
    if (productId === -1) return;

    const api = new Api({ posClientId });
    const transaction = await api.addTransactionLine(
      activeTransactionId,
      productId,
    );
    if (transaction) setActiveTransaction(transaction);
  };

  useEffect(() => {
    getActiveTransaction(posClientId, activeTransactionId);
  }, [posClientId, activeTransactionId]);

  if (!activeTransaction) {
    return <div>Loading...</div>;
  }

  const totalsAmount = activeTransaction.lines.reduce(
    (u, k) => u + k.amount,
    0,
  );

  return (
    <div className="pos">
      {confirmDelete && (
        <ConfirmDialog
          message={localizedStrings.strings["DeleteTransactionConfirmMessage"]}
          onConfirm={deleteTransaction}
          onCancel={() => setConfirmDelete(false)}
        />
      )}
      {paymentMethodSelect && (
        <PaymentMethodSelect
          onSelectPaymentMethod={onPaymentMethodSelected}
          replacers={{ amount: totalsAmount.toString() }}
        ></PaymentMethodSelect>
      )}
      {productSelect && (
        <ProductSelect onSelectProduct={onProductSelected}></ProductSelect>
      )}
      <LineDisplay
        lines={activeTransaction?.lines ?? []}
        onDeleteLine={onDeleteLine}
      />
      <ControlPanel
        transaction={activeTransaction}
        onDeleteTransaction={onDeleteTransaction}
        onFinishTransaction={onPostTransaction}
        onAddProduct={onProductSelection}
      />
    </div>
  );
}

export default Pos;
