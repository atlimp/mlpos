import { useState } from "react";
import "./SingleInputForm.css";
function SingleInputForm({
  label,
  type,
  submitLabel,
  onSubmit,
}: SingleInputFormProps) {
  const [input, setInput] = useState("");

  const submitForm = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(input);
    setInput("");
  };

  return (
    <form className="singleInputForm" onSubmit={submitForm}>
      <label className="singleInputFormLabel">
        {label}:
        <input
          className="singleInputFormInput"
          onInput={(e) => setInput((e.target as HTMLInputElement).value)}
          value={input}
          type={type}
          placeholder={label}
        ></input>
        <button
          className="button buttonPrimary singleInputFormButton"
          type="submit"
        >
          {submitLabel}
        </button>
      </label>
    </form>
  );
}

export default SingleInputForm;
