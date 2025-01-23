import "./Header.css";
import logo from "../../assets/logo.png";
import { SyntheticEvent } from "react";

function Header({ onSelectLanguage, selectedLanguage }: HeaderProps) {
  const supportedLanguages = [
    {
      name: "English",
      code: "en",
    },
    {
      name: "Íslenska",
      code: "is",
    },
  ];

  return (
    <div className="header">
      <img src={logo}></img>
      <select
        value={selectedLanguage}
        className="languageSelect"
        onChange={(e: SyntheticEvent) =>
          onSelectLanguage((e.target as HTMLSelectElement).value)
        }
      >
        {supportedLanguages.map((language) => (
          <option key={language.code} value={language.code}>
            {language.name}
          </option>
        ))}
      </select>
    </div>
  );
}

export default Header;
