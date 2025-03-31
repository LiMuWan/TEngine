using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic
{
    /// <summary>
    /// Utility class to load X509 certificates from StreamingAssets asynchronously.
    /// </summary>
    public static class CertificateLoader
    {
        // Configuration: Consider moving these to a secure configuration file or environment variables.
        private const string CertificateFileName = "gamearena.world.pfx"; // Certificate file name in StreamingAssets
        private const string CertificatePassword = "h1i18b1n"; // Certificate password (Consider securing this)

        // Cache the loaded certificate to prevent multiple loads
        private static X509Certificate2 cachedCertificate = null;

        /// <summary>
        /// Loads the X509Certificate2 from the StreamingAssets directory asynchronously.
        /// Caches the certificate after the first load.
        /// </summary>
        /// <returns>The loaded X509Certificate2, or null if failed.</returns>
        public static async UniTask<X509Certificate2> LoadCertificateAsync()
        {
            if (cachedCertificate != null)
            {
                return cachedCertificate;
            }

            try
            {
                string certPath = Path.Combine(Application.streamingAssetsPath, CertificateFileName);
#if UNITY_EDITOR || UNITY_IOS
                // Check if the certificate file exists
                if (!File.Exists(certPath))
                {
                    Debug.LogError($"Certificate file not found at path: {certPath}");
                    return null;
                }

                // Read the certificate file bytes asynchronously
                byte[] certData = await ReadFileAsync(certPath);
#else
                // Use UnityWebRequest to load the certificate file
                byte[] certData = await LoadCertificateViaUnityWebRequestAsync(certPath);
#endif
                if (certData == null || certData.Length == 0)
                {
                    Debug.LogError($"Failed to load certificate data from path: {certPath}");
                    return null;
                }

                // Initialize the certificate with the loaded data and password
                cachedCertificate = new X509Certificate2(certData, CertificatePassword);

                Debug.Log("Certificate loaded successfully.");
                return cachedCertificate;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading certificate: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Asynchronously reads a file from disk.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>Byte array of file data.</returns>
        private static async UniTask<byte[]> ReadFileAsync(string filePath)
        {
            try
            {
                byte[] data = await File.ReadAllBytesAsync(filePath);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading file at path {filePath}: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Asynchronously loads certificate data using UnityWebRequest.
        /// </summary>
        /// <param name="url">URL to the certificate file.</param>
        /// <returns>Byte array of certificate data, or null if failed.</returns>
        private static async UniTask<byte[]> LoadCertificateViaUnityWebRequestAsync(string url)
        {
            try
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
#if UNITY_2020_1_OR_NEWER
                    await webRequest.SendWebRequest();

                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"UnityWebRequest error: {webRequest.error}");
                        return null;
                    }
#else
                    // For older Unity versions
#pragma warning disable CS0618 // Type or member is obsolete
                    yield return webRequest.SendWebRequest();
                    
                    if (webRequest.isNetworkError || webRequest.isHttpError)
                    {
                        Debug.LogError($"UnityWebRequest error: {webRequest.error}");
                        return null;
                    }
#pragma warning restore CS0618
#endif
                    return webRequest.downloadHandler?.data;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception during UnityWebRequest for {url}: {ex}");
                return null;
            }
        }
    }
}
