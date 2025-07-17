# 🔄 개발 워크플로우 가이드

## 📋 일일 개발 워크플로우

### 1. 작업 시작 전 체크리스트

```bash
# 1. 최신 코드 동기화
git pull origin main

# 2. 의존성 업데이트 확인
dotnet restore

# 3. 프로젝트 빌드 테스트
dotnet build

# 4. 기본 테스트 실행
dotnet test
```

### 2. 새로운 기능 개발 워크플로우

```bash
# 1. 기능 브랜치 생성
git checkout -b feature/new-api-service

# 2. 개발 진행
# - 코드 작성
# - 테스트 작성
# - 문서 업데이트

# 3. 커밋 및 푸시
git add .
git commit -m "feat: add new API service"
git push origin feature/new-api-service

# 4. 풀 리퀘스트 생성
gh pr create --title "Add new API service" --body "Description of changes"
```

### 3. 버그 수정 워크플로우

```bash
# 1. 버그 수정 브랜치 생성
git checkout -b bugfix/api-timeout-issue

# 2. 문제 재현 및 수정
# - 테스트 케이스 작성
# - 버그 수정
# - 회귀 테스트

# 3. 커밋 및 푸시
git add .
git commit -m "fix: resolve API timeout issue"
git push origin bugfix/api-timeout-issue
```

## 🧪 테스트 워크플로우

### 1. 단위 테스트

```bash
# 특정 프로젝트 테스트
dotnet test MyWebApiProject.Tests

# 커버리지 확인
dotnet test --collect:"XPlat Code Coverage"
```

### 2. 통합 테스트

```bash
# API 서버 시작
dotnet run --project MyWebApiProject --urls "http://localhost:5000" &

# 테스트 클라이언트로 통합 테스트
cd TestClient
dotnet run

# 자동화된 통합 테스트
dotnet test MyWebApiProject.IntegrationTests
```

### 3. 성능 테스트

```bash
# 부하 테스트 (예: NBomber 사용)
dotnet run --project LoadTests

# 메모리 사용량 모니터링
dotnet-counters monitor --process-id [PID]
```

## 📦 배포 워크플로우

### 1. 개발 환경 배포

```bash
# 개발 서버 배포
dotnet publish -c Debug -o ./publish/dev
scp -r ./publish/dev/* user@dev-server:/var/www/myapi/

# 또는 Docker 사용
docker build -t myapi:dev .
docker run -p 5000:80 myapi:dev
```

### 2. 프로덕션 배포

```bash
# 프로덕션 빌드
dotnet publish -c Release -o ./publish/prod

# 배포 전 최종 테스트
dotnet test --configuration Release

# 프로덕션 배포
# (실제 배포 스크립트 사용)
```

## 🔧 코드 품질 관리 워크플로우

### 1. 코드 검토 프로세스

```bash
# 코드 포맷팅
dotnet format

# 정적 분석
dotnet analyze

# 보안 취약점 검사
dotnet list package --vulnerable
```

### 2. 자동화된 품질 검사

```yaml
# .github/workflows/quality-check.yml
name: Quality Check
on: [push, pull_request]

jobs:
  quality:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 8.0.x
      - name: Restore dependencies
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore
      - name: Test
        run: dotnet test --no-build --verbosity normal
      - name: Code coverage
        run: dotnet test --collect:"XPlat Code Coverage"
```

## 📈 모니터링 및 유지보수 워크플로우

### 1. 일일 모니터링

```bash
# 로그 확인
tail -f /var/log/myapi/app.log

# 성능 메트릭 확인
curl http://localhost:5000/metrics

# 헬스 체크
curl http://localhost:5000/health
```

### 2. 주간 유지보수

```bash
# 의존성 업데이트 확인
dotnet list package --outdated

# 보안 업데이트
dotnet list package --vulnerable

# 성능 리포트 생성
dotnet run --project PerformanceReports
```

## 🚀 릴리즈 워크플로우

### 1. 릴리즈 준비

```bash
# 1. 릴리즈 브랜치 생성
git checkout -b release/v1.1.0

# 2. 버전 업데이트
# - 프로젝트 파일의 버전 번호 수정
# - CHANGELOG.md 업데이트

# 3. 릴리즈 테스트
dotnet test --configuration Release
```

### 2. 릴리즈 배포

```bash
# 1. 태그 생성
git tag -a v1.1.0 -m "Release version 1.1.0"

# 2. 푸시
git push origin v1.1.0

# 3. GitHub 릴리즈 생성
gh release create v1.1.0 --title "Version 1.1.0" --notes "Release notes"
```

## 🔄 핫픽스 워크플로우

### 1. 긴급 수정 프로세스

```bash
# 1. 핫픽스 브랜치 생성 (main에서)
git checkout main
git pull origin main
git checkout -b hotfix/critical-security-fix

# 2. 빠른 수정 및 테스트
# - 최소한의 변경
# - 철저한 테스트

# 3. 즉시 배포
git add .
git commit -m "hotfix: critical security vulnerability"
git push origin hotfix/critical-security-fix

# 4. 긴급 머지 및 배포
git checkout main
git merge hotfix/critical-security-fix
git tag -a v1.0.1 -m "Hotfix version 1.0.1"
git push origin main --tags
```

## 📊 성과 추적 워크플로우

### 1. 개발 메트릭 수집

```bash
# 커밋 통계
git log --oneline --since="1 week ago" | wc -l

# 코드 변경 통계
git diff --stat HEAD~7 HEAD

# 테스트 커버리지 추적
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"coverage.xml" -targetdir:"coverage-report"
```

### 2. 성능 메트릭 추적

```bash
# API 응답 시간 모니터링
curl -w "%{time_total}\n" -o /dev/null -s http://localhost:5000/chat

# 메모리 사용량 추적
dotnet-counters monitor --process-id [PID] --counters System.Runtime
```

## 🎯 팀 협업 워크플로우

### 1. 코드 리뷰 프로세스

**리뷰어 체크리스트:**
- [ ] 코드 스타일 일관성
- [ ] 테스트 코드 포함
- [ ] 문서 업데이트
- [ ] 성능 영향 검토
- [ ] 보안 취약점 확인

### 2. 스탠드업 미팅 템플릿

**일일 스탠드업:**
- 어제 완료한 작업
- 오늘 진행할 작업
- 블로커 및 도움 요청

### 3. 스프린트 관리

```bash
# 스프린트 시작
git checkout -b sprint/2024-01-week3

# 작업 브랜치 관리
git checkout -b feature/user-authentication
git checkout -b feature/api-rate-limiting

# 스프린트 완료
git checkout main
git merge sprint/2024-01-week3
```

## 🔐 보안 워크플로우

### 1. 보안 검사 프로세스

```bash
# 의존성 취약점 검사
dotnet list package --vulnerable

# 코드 보안 검사 (예: SonarQube)
dotnet sonarscanner begin /k:"myapi"
dotnet build
dotnet sonarscanner end

# 시크릿 스캔
git secrets --scan
```

### 2. API 키 순환 프로세스

```bash
# 1. 새 API 키 생성
# 2. 설정 파일 업데이트
# 3. 배포 및 테스트
# 4. 이전 키 비활성화
# 5. 모니터링
```

## 📋 체크리스트 모음

### 새로운 기능 개발 체크리스트

- [ ] 기능 요구사항 분석
- [ ] 설계 문서 작성
- [ ] 코드 구현
- [ ] 단위 테스트 작성
- [ ] 통합 테스트 작성
- [ ] 코드 리뷰 완료
- [ ] 문서 업데이트
- [ ] 성능 테스트
- [ ] 보안 검토
- [ ] 배포 준비

### 릴리즈 체크리스트

- [ ] 모든 테스트 통과
- [ ] 코드 커버리지 기준 충족
- [ ] 보안 검사 완료
- [ ] 성능 테스트 완료
- [ ] 문서 업데이트 완료
- [ ] 배포 스크립트 테스트
- [ ] 롤백 계획 수립
- [ ] 모니터링 설정 확인

### 긴급 대응 체크리스트

- [ ] 문제 상황 파악
- [ ] 영향 범위 분석
- [ ] 임시 해결책 적용
- [ ] 근본 원인 분석
- [ ] 영구 해결책 구현
- [ ] 테스트 및 검증
- [ ] 배포 및 모니터링
- [ ] 사후 분석 및 개선

---

**마지막 업데이트:** 2025년 1월 17일  
**버전:** 1.0.0  
**작성자:** Claude Code Assistant