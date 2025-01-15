import { Dispatch, createContext, SetStateAction } from "react";

export interface PosClientIdContextInterface {
  posClientId: number;
  setPosClientId: Dispatch<SetStateAction<number>>;
}
export interface TransactionContextInterface {
  activeTransactionId: number;
  setActiveTransactionId: Dispatch<SetStateAction<number>>;
}

const PosClientIdContextDefaultState = {
  posClientId: -1,
  setPosClientId: () => {},
} as PosClientIdContextInterface;

const TransactionContextDefaultState = {
  activeTransactionId: -1,
  setActiveTransactionId: () => {},
} as TransactionContextInterface;

export const TransactionContext = createContext(TransactionContextDefaultState);
export const PosClientIdContext = createContext(PosClientIdContextDefaultState);
