using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Library.Logger;
using log4net;
using System.Reflection;

namespace Library.Helper.Firebase;

public class FirebaseStorageHelper
{
    private static readonly Lazy<FirebaseStorageHelper> lazy = new Lazy<FirebaseStorageHelper>(() => new FirebaseStorageHelper());
    public static FirebaseStorageHelper Instance { get { return lazy.Value; } }

    private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
    private StorageClient _storageClient = null!;
    private readonly object _padlock = new object();
    private string _jsonKeyPath = ConstInfo.FirebaseStorageCredintial;
    private string _bucketName = ConstInfo.FirebaseStorageBucketName;
    

    public FirebaseStorageHelper()
    {
        lock (_padlock)
        {
            if (_storageClient == null)
            {
                // 서비스 계정 키 파일에서 인증 정보를 로드
                GoogleCredential credential = GoogleCredential.FromFile(_jsonKeyPath);
                _storageClient = StorageClient.Create(credential);
            }
        }
    }
    public void DeleteFile(string objectPath)
    {
        try
        {
            _storageClient.DeleteObject(_bucketName, objectPath);
            _logger.InfoEx(() => $"✅ Succeed to Delete File : {_bucketName}/{objectPath}");
        }
        catch (Exception ex)
        {
            _logger.Error($"❌ Fail to Delete File : {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string objectPath)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(_bucketName, objectPath);
            _logger.InfoEx(() => $"✅ Succeed to Delete File : {_bucketName}/{objectPath}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"❌ Fail to Delete File : {ex.Message}", ex);
            return false;
        }
    }
}
