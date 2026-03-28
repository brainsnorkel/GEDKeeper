## 1. Theme Rendering Fixes (Critical)

- [x] 1.1 Add runtime guard around TabPage BackgroundColor assignment in `ThemeTabPageHandler` (`EtoThemeManager.cs:524-528`) -- uses `RuntimeInformation.IsOSPlatform(OSPlatform.OSX)`
- [x] 1.2 Add runtime guard around TabControl BackgroundColor assignment in `ThemeTabControlHandler` (`EtoThemeManager.cs:496-500`) -- uses `RuntimeInformation.IsOSPlatform(OSPlatform.OSX)`
- [x] 1.3 Update `GKTabControlHandler` to return `SystemFonts.Default()` instead of null for Font property (`EtoHandlers.cs:404`)
- [x] 1.4 Update `GKContextMenuHandler` to return `SystemFonts.Default()` instead of null for Font property (`EtoHandlers.cs:372`)
- [x] 1.5 Update `GKToolBarHandler` to return `SystemColors.Control`/`SystemColors.ControlText` instead of `Colors.Transparent` (`EtoHandlers.cs:350-362`)
- [x] 1.6 Update `GKMenuBarHandler` to return `SystemColors.Control`/`SystemFonts.Default()`/`SystemColors.ControlText` (`EtoHandlers.cs:382-394`)
- [ ] 1.7 Verify tab labels render cleanly on macOS after theme application -- test with all built-in themes

## 2. Font Disposal Fix

- [ ] 2.1 Analyze Eto.Mac source to understand the NSFont shared disposal path and identify exact failure conditions
- [x] 2.2 Implement reference-counting wrapper for shared font family tracking in `GKFontHandler` (`EtoHandlers.cs:281-344` -- `AddRef`/`Release`/`fFontRefCounts` dictionary)
- [x] 2.3 Replace `DisposeControl() => false` with reference-aware disposal that protects only actively-shared fonts (`EtoHandlers.cs:332-343`)
- [ ] 2.4 Add periodic cleanup sweep for zero-reference fonts older than 60 seconds (may not be needed -- `DisposeControl` already handles on GC)
- [ ] 2.5 Test font memory behavior: open/close multiple forms, verify no font corruption and reduced memory leaks

## 3. Grid Cell Formatting on macOS

- [x] 3.1 **Spike**: GridCellFormat feature flag now enabled on macOS. `OnRowFormatting`/`OnCellFormatting` will execute. Grid text color fixed to `SystemColors.ControlText` (`GKGridView.cs:25`), row background to `SystemColors.ControlBackground` (`GKListView.cs:130`). CustomCell Paint API confirmed available on Mac64 as fallback. **Runtime testing needed**: if Eto.Mac64 fires formatting events, CustomCell is unnecessary.
- [ ] 3.2 Create a `MacGridCellPainter` class (contingent on 3.1 runtime test failure -- only if Eto.Mac64 doesn't fire formatting events)
- [ ] 3.3 Integrate `MacGridCellPainter` into `GKSheetList` / `GKGridView` (contingent on 3.2)
- [x] 3.4 `EtoAppHost.HasFeatureSupport` returns `true` for GridCellFormat on all platforms (`EtoAppHost.cs:252-253`). Note: the real issue is Eto's `OnCellFormatting` event not firing reliably on Mac64, which is what tasks 3.1-3.3 address.
- [ ] 3.5 Benchmark scrolling performance with CustomCell Paint on a 10,000-row record list on Intel Mac
- [ ] 3.6 Verify conditional formatting displays correctly: deceased individuals highlighted, incomplete records marked

## 4. Dark Mode Support

- [x] 4.1 Add appearance change observation via SystemColors polling timer (`EtoAppHost.cs:440-454` -- 2s interval, hash-based detection)
- [x] 4.2 Create "System" theme with all colors from SystemColors (`EtoThemeManager.cs:CreateSystemThemeElements()` -- replaces 8 hardcoded colors with SystemColors equivalents)
- [x] 4.3 Wire appearance change notification to trigger theme refresh via `RefreshDefaultTheme()` + `ApplyTheme()` (`EtoAppHost.cs:456-466`)
- [ ] 4.4 Update `ChartRenderer` color constants -- base class has no hardcoded colors (no action needed); GKCore chart models use GKColors constants (out of scope per guardrails)
- [x] 4.5 Update `TreeChartBox` background to `SystemColors.ControlBackground` (`TreeChartBox.cs:161`). Printer backgrounds kept as `Colors.White` (paper).
- [ ] 4.6 Update `CircleChart` -- uses `fModel.Options.BrushColor[]` from options (already theme-aware). Printer `Colors.White` is correct. Deferred: deeper color changes require GKCore CircleChartOptions changes.
- [x] 4.7 Update `ArborViewer` background, text brushes, and edge colors to use SystemColors (`ArborViewer.cs:100,119-120,160-172`)
- [ ] 4.8 Test Dark Mode: switch system appearance while app is running, verify all windows and charts update within 1 second
- [ ] 4.9 Test custom theme override: verify non-System themes are unaffected by system appearance changes

## 5. macOS Catalina (10.15) Compatibility

- [x] 5.1 Add `<SupportedOSPlatformVersion>10.15</SupportedOSPlatformVersion>` to `GEDKeeper3.csproj` macOS configuration (`GEDKeeper3.csproj:109`)
- [x] 5.2 Set `LSMinimumSystemVersion` to `10.15` in Info.plist (`Info.plist:7-8`)
- [ ] 5.3 Audit all `#if OS_MACOS` code paths for API availability -- grep for any macOS 11.0+ only APIs
- [ ] 5.4 Add `[SupportedOSPlatformGuard]` attributes where newer APIs are used with fallbacks
- [ ] 5.5 Verify .NET 8 self-contained runtime in .app bundle works on macOS 10.15 (check runtime compatibility matrix)
- [ ] 5.6 Verify Eto.Platform.Mac64 2.10.0 minimum macOS version support
- [ ] 5.7 Build and test osx-x64 binary on macOS 10.15 (Catalina) -- launch, open GEDCOM, edit record, save
- [ ] 5.8 Build and test osx-arm64 binary on macOS 11+ Apple Silicon -- verify native execution
- [ ] 5.9 Test osx-arm64 binary under Rosetta 2 on Intel Mac (macOS 11+) -- verify no crashes

## 6. Native Polish

- [x] 6.1 Set default UI font to `.AppleSystemUIFont` (San Francisco) on macOS (`EtoThemeManager.cs:36-38` via `GetDefaultFontFamily()`)
- [x] 6.2 Fix `ScrollablePanel.OnShown()` cross-platform behavior -- replaced try-catch with `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` guard (`ScrollablePanel.cs:257-260`)
- [ ] 6.3 Audit macOS menu bar against HIG: ensure GEDKeeper app menu has About, Preferences (Cmd+,), Services, Hide, Quit
- [ ] 6.4 Verify standard keyboard shortcuts: Cmd+C/V/X/Z/A, Cmd+, for Preferences, Cmd+Q for Quit
- [ ] 6.5 Verify Retina display rendering: check ChartRenderer accounts for display scale factor in line widths, font sizes, and bitmap creation
- [ ] 6.6 Verify toolbar and menu icons provide @2x variants or use vector rendering for Retina displays
- [ ] 6.7 Verify file open/save dialogs use native NSOpenPanel/NSSavePanel with GEDCOM file type filtering (.ged, .gedcom)
- [x] 6.8 Add GEDCOM file type associations in Info.plist (`Info.plist:17-30` -- CFBundleDocumentTypes for .ged/.gedcom)

## 7. Integration Testing and Validation

- [ ] 7.1 Full test pass on macOS 10.15 (Catalina): launch, open file, navigate all tabs, edit records, view charts, save, close
- [ ] 7.2 Full test pass on latest macOS: same test matrix as 7.1
- [ ] 7.3 Verify Windows build is unaffected: run existing test suite, visual spot-check
- [ ] 7.4 Verify Linux build is unaffected: run existing test suite
- [ ] 7.5 Performance comparison: measure startup time, scrolling FPS, memory usage before and after changes on Intel Mac
- [ ] 7.6 Document all visual changes in release notes draft
