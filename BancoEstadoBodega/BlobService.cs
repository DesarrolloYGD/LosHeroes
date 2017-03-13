using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types

namespace BancoEstadoBodega
{
    public class BlobService
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //metodo para subir o publicar blobs dependiendo de una clave identificatoria
        public void AddImgProducto(HttpPostedFileBase imagen, string id_imgProducto)
        {
            try
            {
                CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();//creaciòn del cliente blob para la cuenta definida en el web.config
                CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");//especificaciòn del contenedor que almacena los blobs
                CloudBlockBlob blockBlob = contenedor.GetBlockBlobReference(id_imgProducto);//metodo para referenciar el blob que se crearà en el contenedor
                blockBlob.Properties.ContentType = "image/jpeg";//se define el tipo de contenido del blob
                blockBlob.UploadFromStream(imagen.InputStream);//se sube el blob a la nube
            }
            catch (NullReferenceException e) { };

        }

        public byte[] GetImgProducto(string id_imgProducto)
        {
            CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");
            CloudBlockBlob blockBlob = contenedor.GetBlockBlobReference(id_imgProducto);
            blockBlob.FetchAttributes();
            long fileByteLength = blockBlob.Properties.Length;
            Byte[] auxiliar = new Byte[fileByteLength];
            blockBlob.DownloadToByteArray(auxiliar, 0);
            return auxiliar;
        }


        public void AddPDFSol(HttpPostedFileBase pdf, string id_pdfsol)
        {
            try
            {
                CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();//creaciòn del cliente blob para la cuenta definida en el web.config
                CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");//especificaciòn del contenedor que almacena los blobs
                CloudBlockBlob blockBlob = contenedor.GetBlockBlobReference(id_pdfsol);//metodo para referenciar el blob que se crearà en el contenedor
                blockBlob.Properties.ContentType = "application/pdf";//se define el tipo de contenido del blob
                blockBlob.UploadFromStream(pdf.InputStream);//se sube el blob a la nube
            }
            catch (NullReferenceException e) { };

        }

        public byte[] GetPDFSol(string id_pdfsol)
        {
            CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");
            CloudBlockBlob blockBlob = contenedor.GetBlockBlobReference(id_pdfsol);
            blockBlob.FetchAttributes();
            long fileByteLength = blockBlob.Properties.Length;
            Byte[] auxiliar = new Byte[fileByteLength];
            blockBlob.DownloadToByteArray(auxiliar, 0);
            return auxiliar;
        }
        public void EliminarImgProducto(string id_imgProducto)
        {
            CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");
            CloudBlockBlob blockBlob = contenedor.GetBlockBlobReference(id_imgProducto);
            blockBlob.DeleteIfExists();
        }

        public string obtenerDatos()
        {
            
            CloudBlobClient cliente = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer contenedor = cliente.GetContainerReference("losheroesblob");
            String cadena = "https://pruebasmarco.blob.core.windows.net//" + contenedor.Name + "/" ;
            return cadena;

        }



    }

}