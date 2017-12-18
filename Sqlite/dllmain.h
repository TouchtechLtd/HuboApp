// dllmain.h : Declaration of module class.

class CSqliteModule : public ATL::CAtlDllModuleT< CSqliteModule >
{
public :
	DECLARE_LIBID(LIBID_SqliteLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_SQLITE, "{C5BB134F-D29C-46E2-8525-6B699AD03CAD}")
};

extern class CSqliteModule _AtlModule;
