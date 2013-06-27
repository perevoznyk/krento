!INCLUDE ..\delphi.mak
!INCLUDE ..\framework.mak

!IF "$(DELPHI_CODE)" == "D5"
package = GDIPlusR5
!ENDIF

!IF "$(DELPHI_CODE)" == "D2006"
package = GDIPlusR10
!ENDIF

!IF "$(DELPHI_CODE)" == "D2007"
package = GDIPlusR11
!ENDIF

makdir = $(MAKEDIR)
outdir = $(makdir)\Lib\$(DELPHI_CODE)
srcdir = $(makdir)
pkgdir = $(makdir)
bpldir = $(DELPHI_PATH_BPL:"=)

all: clean_old clean build

clean_old:
	if exist "$(outdir).bat" del /q "$(outdir).bat"

clean:
	if exist "$(outdir)\$(NULL)" rmdir /s /q "$(outdir)"
	if exist "$(bpldir)\$(package).bpl" del /q "$(bpldir)\$(package).bpl"

build: outdir bpl

outdir:
	if not exist "$(outdir)\$(NULL)" mkdir "$(outdir)"

dcc_args = -B -N"$(outdir)" -LE"$(bpldir)" -LN"$(outdir)"

bpl:
	cd "$(srcdir)"
	$(DELPHI_DCC32) $(dcc_args) "$(pkgdir)\$(package).dpk"
	cd "$(makdir)"
