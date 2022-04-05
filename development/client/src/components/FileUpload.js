import React, { useState, useContext } from "react";
import { uploadImg } from "../api/user.api";
import { globalContext } from "../store/globalContext";

export const FileUpload = ({ cuser }) => {
  const { setCurrentUser } = useContext(globalContext);

  const [file, setFile] = useState();
  const [fileName, setFileName] = useState();

  const saveFile = (e) => {
    console.log(e.target.files[0]);
    setFile(e.target.files[0]);
    setFileName(e.target.files[0].name);
  };

  const uploadFile = async (e) => {
    console.log(file);
    const formData = new FormData();
    formData.append("file", file);
    formData.append("fileName", fileName);
    let form_obj = {
      formData,
      cuser,
    };

    uploadImg(form_obj, (err, result) => {
      if (err) {
        console.log(err);
      } else {
        setCurrentUser({
          ...cuser,
          imgUrl: result.data.imgUrl,
          imgPublicId: result.data.imgPublicId,
        });
      }
    });
  };

  return (
    <>
      <input
        type="file"
        onChange={saveFile}
        style={{
          width: "100px",
          height: "100px",
        }}
      />
      <input type="button" value="upload" onClick={uploadFile} />
    </>
  );
};
