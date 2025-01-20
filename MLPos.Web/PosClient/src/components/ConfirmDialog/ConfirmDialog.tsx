import "./ConfirmDialog.css";

import { useContext } from "react";
import { LocalizedStringsContext } from "../../context";
function ConfirmDialog({ message, onConfirm, onCancel }: ConfirmDialogProps) {
  const { localizedStrings } = useContext(LocalizedStringsContext);

  return (
    <div className="modal">
      <div className="modalContent confirmDialog">
        <div className="confirmDialogMessage">{message}</div>
        <div className="confirmDialogButtons">
          <button
            className="button buttonPrimary confirmButton"
            onClick={onConfirm}
          >
            {localizedStrings.strings["Confirm"]}
          </button>
          <button
            className="button buttonSecondary cancelButton"
            onClick={onCancel}
          >
            {localizedStrings.strings["Cancel"]}
          </button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmDialog;
